using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using NPOI.SS.Formula.Functions;
using Remotion.FunctionalProgramming;


namespace DataAggregator.Core.Classifier
{
    public class ClassifierEditor
    {
        private readonly DrugClassifierContext _context;

        public ClassifierEditor(DrugClassifierContext context)
        {
            _context = context;
        }

        #region Action

        /// <summary>
        /// Проверяем что новое а что старое
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ClassifierInfoModel CheckClassifier(ClassifierEditorModelJson model)
        {
            model.Clear();

            ClassifierInfoModel infoModel = new ClassifierInfoModel();

            var newWord = "Новый";

            var drug = CheckDrug(model);
            var ownerTradeMark = FindManufacturer(model.OwnerTradeMark);
            var packer = FindManufacturer(model.Packer);
            Manufacturer ownerRegCert = null;
            if (model.SelectedRegistrationCertificate != null)
                ownerRegCert = FindManufacturer(
                    model.OwnerRegistrationCertificate);

            infoModel.LKCU = drug != null ? drug.LKCU.ToString() : newWord;
            infoModel.OwnerTradeMarkKey = ownerTradeMark != null ? ownerTradeMark.Key : newWord;
            infoModel.PackerKey = packer != null ? packer.Key : newWord;

            infoModel.OwnerRegistrationCertificateKey = ownerRegCert != null ? ownerRegCert.Key : newWord;

            return infoModel;
        }
        


        public struct ChangeStatus
        {
            public bool CanRecreate { get; set; }

            public bool NeedMerge { get; set; }

            public string DrugDescription { get; set; }

            public bool OnlyOneProductionInfo { get; set; }
        }

        /// <summary>
        /// Проверяем нужно ли делать объединение, что сохранить с новым LKCU 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChangeStatus CheckRecreate(ClassifierEditorModelJson model)
        {

          

            model.Clear();

            ChangeStatus changeStatus = new ChangeStatus()
            {
                CanRecreate = true,
                NeedMerge = false,
                DrugDescription = string.Empty,
                OnlyOneProductionInfo = false
            };

            var currentDrug = _context.Drugs.First(d => d.Id == model.DrugId);

            changeStatus.OnlyOneProductionInfo = currentDrug.ProductionInfo.Count == 1;


            var drugProperty = GetDrugPropery(model);

            //Ищем Drug
            Drug drug = null;

            // Ищем если
            if (drugProperty.IsNew)
                return changeStatus;

            drug = FindDrug(drugProperty);

            if (drug != null && drug.Id == model.DrugId)
                changeStatus.CanRecreate = false;

            if (drug != null)
            {

                changeStatus.NeedMerge = true;

                StringBuilder builder = new StringBuilder();
                builder.Append("LKCU : ");
                builder.Append(drug.LKCU + " ");
                builder.Append(drug.TradeName.Value + " ");
                if (drug.INNGroup != null)
                    builder.Append(drug.INNGroup.Description);
                if (drug.FormProduct != null)
                    builder.Append(drug.FormProduct.Value + " ");
                if (drug.DosageGroup != null)
                    builder.Append(drug.DosageGroup.Description + " ");
                builder.Append("N" + drug.ConsumerPackingCount);

                changeStatus.DrugDescription = builder.ToString();
            }


            return changeStatus;

            

        }

        /// <summary>
        /// Сохранение с новым LKCU c объединением
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tryMode"></param>
        /// <returns></returns>
        public ClassifierInfoModel MergeClassifier(ClassifierEditorModelJson model, bool tryMode)
        {

            ClassifierInfoModel info = new ClassifierInfoModel();

            var transaction = _context.Database.BeginTransaction();

            model.Clear();

            var fromDrug = _context.Drugs.First(d => d.Id == model.DrugId);

            var drugProperty = GetDrugPropery(model);
            var toDrug = FindDrug(drugProperty);

            if (toDrug == null)
                throw new ApplicationException("toDrug is null need ReCreate");

            var ownerTradeMark = FindManufacturer(model.OwnerTradeMark) ?? CreateManufacturer(model.OwnerTradeMark);
            var packer = FindManufacturer(model.Packer) ?? CreateManufacturer(model.Packer);


            var fromProductionInfo = FindProductionInfo(ownerTradeMark, packer, fromDrug);

            var toProductionInfo = FindProductionInfo(ownerTradeMark, packer, toDrug) ?? CreateProductionInfo(model, toDrug, ownerTradeMark, packer);


            foreach (var drugPackingJson in model.DrugPackings)
            {
                var drugPacking = FindDrugPacking(toDrug, drugPackingJson) ?? CreateDrugPacking(toDrug, drugPackingJson);

                if (toProductionInfo.DrugPacking.All(d => d.Id != drugPacking.Id))
                    toProductionInfo.DrugPacking.Add(drugPacking);
            }


            foreach (var registrationCertificateJson in model.RegistrationCeritifactes)
            {
                var registrationCertificate = FindCertificate(registrationCertificateJson);

                if (toProductionInfo.RegistrationCertificate.All(d => d.Id != registrationCertificate.Id))
                    toProductionInfo.RegistrationCertificate.Add(registrationCertificate);
            }

            //Значит такой ProductionInfo не было и мы делаем merge на другой
            if (fromProductionInfo != null)
            {

                //Удаляем из существующего классификатора
                RemoveProductionInfoDromDrug(fromDrug, ownerTradeMark, packer);

                //Теперь переносим связанные данные
                ClassifierInfo from = new ClassifierInfo()
                {
                    DrugId = fromDrug.Id,
                    OwnerTradeMarkId = ownerTradeMark.Id,
                    PackerId = packer.Id
                };

                ClassifierInfo to = new ClassifierInfo()
                {
                    DrugId = toDrug.Id,
                    OwnerTradeMarkId = ownerTradeMark.Id,
                    PackerId = packer.Id
                };

                ReClassifier(from, to);

            }

           
            _context.SaveChanges();
            transaction.Commit();

            info.DrugId = toDrug.Id;
            info.PackerId = packer.Id;
            info.OwnerTradeMarkId = ownerTradeMark.Id;
            info.Used = toProductionInfo.Used;


            return info;
        }
        
        /// <summary>
        /// Сохранить с новым LKCU без объединения
        /// </summary>
        public ClassifierInfoModel ReCreateClassifier(ClassifierEditorModelJson model, bool tryMode)
        {

            var transaction = _context.Database.BeginTransaction();

            model.Clear();

            var existDrug = _context.Drugs.FirstOrDefault(d => d.Id == model.DrugId);

            if (existDrug == null)
                throw new ApplicationException("Drug not found");


            var drugProperty = GetDrugPropery(model);

            //Ищем Drug
            Drug drug = FindDrug(drugProperty);

            if (drug != null)
                throw new ApplicationException("Данный Drug уже существует нужно воспользоваться объединением");

            // Ищем если

            //Нужно добавить, чтобы перенеслись все регисрационные сертификаты, а не только выбранный.

            ClassifierInfoModel info = AddClassifier(model, false);

            var productionInfo = info.ProductionInfo;


            foreach (var registrationCertificateJson in model.RegistrationCeritifactes)
            {
                var regCert = FindCertificate(registrationCertificateJson);

                if (regCert != null && productionInfo.RegistrationCertificate.All(r => r.Id != regCert.Id))
                    productionInfo.RegistrationCertificate.Add(regCert);
            }

            var ownerTradeMark = FindManufacturer(model.OwnerTradeMark) ?? CreateManufacturer(model.OwnerTradeMark);
            var packer = FindManufacturer(model.Packer) ?? CreateManufacturer(model.Packer);

            //Удаляем из существующего классификатора

            RemoveProductionInfoDromDrug(existDrug, ownerTradeMark, packer);


            //Теперь переносим связанные данные

            ClassifierInfo from = new ClassifierInfo()
            {
                DrugId = model.DrugId,
                OwnerTradeMarkId = ownerTradeMark.Id,
                PackerId = packer.Id
            };

            ClassifierInfo to = new ClassifierInfo()
            {
                DrugId = info.DrugId.Value,
                OwnerTradeMarkId = info.OwnerTradeMarkId,
                PackerId = info.PackerId
            };


          


            ReClassifier(from, to);

           
            _context.SaveChanges();

            transaction.Commit();

            info.DrugId = info.DrugId.Value;
            info.PackerId = info.PackerId;
            info.OwnerTradeMarkId = info.OwnerTradeMarkId;
            info.Used = info.ProductionInfo.Used;


            return info;

        }
        
        /// <summary>
        /// Удаляем ProductionInfo из старого Drug после сохранения с новым LKCU 
        /// </summary>
        /// <param name="drug"></param>
        /// <param name="ownerTradeMark"></param>
        /// <param name="packer"></param>
        private void RemoveProductionInfoDromDrug(Drug drug, Manufacturer ownerTradeMark, Manufacturer packer)
        {
            DeleteProductionInfo(drug, ownerTradeMark, packer);

            foreach (var drugPacking in drug.DrugPacking)
            {
                if (drugPacking.ProductionInfo.Count == 0)
                    DeleteDrugPacking(drugPacking);
            }

            if (drug.ProductionInfo.Count == 0)
                throw new ApplicationException("Нельзя удалять Drug");
            //DeleteDrug(drug);
        }


     

        public class ChangeDescription
        {
            public string Title { get; set; }

            public string OldId { get; set; }

            public string NewId { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get;set; }

            public ChangeDescription(string title, string oldValue, string newValue)
            {
                this.OldValue = oldValue;
                this.NewValue = newValue;
                this.Title = title;
            }

            public ChangeDescription(string title, long oldId, string oldValue,  long newId, string newValue)
            {
                this.NewId = newId == 0 ? "Новый" : newId.ToString();
                this.OldId = oldId == 0 ? "Новый" : oldId.ToString();
                this.OldValue = oldValue;
                this.NewValue = newValue;
                this.Title = title;
            }

            public string NewText
            {
                get { return String.Format("{0} {1}", NewId, NewValue).Trim(); }
            }

            public string OldText
            {
                get { return String.Format("{0} {1}", OldId, OldValue).Trim(); }
            }
        }

        public class ChangeInfo
        {
            public List<ChangeDescription> Items { get; set; }
            public List<ChangeDescription> RealPackingCount { get; set; }
            public List<ChangeDescription> DrugPacking { get; set; } 
        }

        public ChangeInfo GetChanges(ClassifierEditorModelJson model)
        {

            model.Clear();

            List<ChangeDescription> changes = new List<ChangeDescription>();
            List<ChangeDescription> realPackingCount = new List<ChangeDescription>();
            List<ChangeDescription> drugPacking = new List<ChangeDescription>();

            

            var drug = _context.Drugs.First(d => d.Id == model.DrugId);
            
            //Изменяемый Drug не найден
            if (drug == null)
                throw new ApplicationException(string.Format("not found changes Drug Id = {0}", model.DrugId));

            var drugProperty = GetDrugPropery(model);

            //Сравниваем INN
            if ((drug.INNGroup != null && drugProperty.INNGroup == null) ||
                 (drug.INNGroup == null && drugProperty.INNGroup != null) ||
                 (drug.INNGroup != null && drugProperty.INNGroup != null && drug.INNGroup.Description != drugProperty.INNGroup.Description))
            {

                var oldValue = String.Empty;
                if(drug.INNGroup != null)
                    oldValue = drug.INNGroup.Description;

                var newValue = String.Empty;
                if (drugProperty.INNGroup != null)
                    newValue = drugProperty.INNGroup.Description;

                changes.Add(new ChangeDescription("МНН", oldValue, newValue));
            }


            if  (   (drug.DosageGroup != null && drugProperty.DosageGroup == null) ||
                    (drug.DosageGroup == null && drugProperty.DosageGroup != null) ||
                    (drug.DosageGroup != null && drugProperty.DosageGroup != null && drug.DosageGroup.Description != drugProperty.DosageGroup.Description))
            {
                var oldValue = String.Empty;
                if (drug.DosageGroup != null)
                    oldValue = drug.DosageGroup.Description;

                var newValue = String.Empty;
                if (drugProperty.DosageGroup != null)
                    newValue = drugProperty.DosageGroup.Description;

                changes.Add(new ChangeDescription("Дозировка", oldValue, newValue));
            }

            if (drug.TradeName.Value != drugProperty.TradeName.Value)
            {
                changes.Add(new ChangeDescription("Торговое наименование", drug.TradeName.Value, drugProperty.TradeName.Value));
            }

            if (drug.FormProduct.Value != drugProperty.FormProduct.Value)
            {
                changes.Add(new ChangeDescription("Лекарственная форма", drug.FormProduct.Value, drugProperty.FormProduct.Value));
            }

            if (drug.ConsumerPackingCount != model.ConsumerPackingCount)
            {
                changes.Add(new ChangeDescription("Фасовка", drug.ConsumerPackingCount.ToString(), model.ConsumerPackingCount.ToString()));
            }

            if (drug.DrugType.Id != drugProperty.DrugTypeId)
            {
                var newValue = _context.DrugType.First(t => t.Id == drugProperty.DrugTypeId).Value;
                changes.Add(new ChangeDescription("Тип", drug.DrugType.Value, newValue));
            }


            var ownerTradeMark = FindManufacturer(model.OwnerTradeMark) ?? new Manufacturer() { Key = "Новый", Value = model.OwnerTradeMark.Value, Id = 0 };
            var packer = FindManufacturer(model.Packer) ?? new Manufacturer() { Key = "Новый", Value = model.Packer.Value, Id = 0 };
            var oldOwnerTradeMark = _context.Manufacturer.First(m => m.Id == model.OwnerTradeMarkId);
            var oldPacker = _context.Manufacturer.First(m => m.Id == model.PackerId);

            if (oldOwnerTradeMark.Value != ownerTradeMark.Value)
            {
                changes.Add(new ChangeDescription("Правообладатель", oldOwnerTradeMark.Value, ownerTradeMark.Value));
            }

            if (oldPacker.Value != packer.Value)
            {
                changes.Add(new ChangeDescription("Упаковщик", oldOwnerTradeMark.Value, ownerTradeMark.Value));
            }

            List<int> oldRealPackingCount = drug.RealPacking.Select(r => r.RealPackingCount).ToList();
          
            List<int> newRealPacking = new List<int>();
            if (model.RealPackingList != null)
                newRealPacking = model.RealPackingList.Select(c => c.RealPackingCount).ToList();
            List<int> add = newRealPacking.Except(oldRealPackingCount).ToList();
            List<int> delete = oldRealPackingCount.Except(newRealPacking).ToList();

            foreach (var i in add)
            {
                realPackingCount.Add(new ChangeDescription("", String.Empty, i.ToString()));
            }

            foreach (var i in delete)
            {
                realPackingCount.Add(new ChangeDescription("",  i.ToString(), String.Empty));
            }

            var productionInfo = _context.ProductionInfo.First(p => p.Id == model.ProductionInfoId);

            if (productionInfo.Used != model.Used)
            {
                var oldUsed = productionInfo.Used ? "использовать" : "заблокирован";
                var newUsed = model.Used ? "использовать" : "заблокирован";

                changes.Add(new ChangeDescription("Блокировка", oldUsed, newUsed));
            }

            List<string> oldPacking = productionInfo.DrugPacking.Select(GetDesription).ToList();
            List<string> newPacking = new List<string>();
            if (model.DrugPackings != null)
                newPacking = model.DrugPackings.Select(GetDesription).ToList();

         
            foreach (var pack in newPacking)
            {
                if (oldPacking.All(p => !string.Equals(p, pack)))
                {
                    drugPacking.Add(new ChangeDescription("", String.Empty, pack));
                }
            }

            foreach (var pack in oldPacking)
            {
                if (newPacking.All(p => !string.Equals(p, pack)))
                {
                    drugPacking.Add(new ChangeDescription("", pack, String.Empty));
                }
            }

   

            return new ChangeInfo() { Items = changes, RealPackingCount = realPackingCount, DrugPacking = drugPacking};

            

            //Реальные упаковки
            //Упаковки
        }


        public bool ChangeRealPacking(ClassifierEditorModelJson model)
        {
            var transaction = _context.Database.BeginTransaction();

            var realPackingIdExists = _context.RealPacking.Where(r => r.DrugId == model.DrugId).ToList();

            foreach (var realPack in realPackingIdExists)
            {
                if (model.RealPackingList == null || model.RealPackingList.All(d => d.Id != realPack.Id))
                    _context.RealPacking.Remove(realPack);
            }

            if (model.RealPackingList != null)
            {
                foreach (var pack in model.RealPackingList.Where(r => r.Id == 0))
                {
                    _context.RealPacking.Add(new RealPacking()
                    {
                        DrugId = model.DrugId,
                        RealPackingCount = pack.RealPackingCount
                    });
                }
            }

            _context.SaveChanges();
            transaction.Commit();

            return true;
        }


        //Изменить 
        public ClassifierInfoModel ChangeClassifier(ClassifierEditorModelJson model, bool tryMode)
        {
            var transaction = _context.Database.BeginTransaction();

            model.Clear();

            ClassifierInfoModel info = new ClassifierInfoModel();

            var drug = _context.Drugs.FirstOrDefault(d => d.Id == model.DrugId);

            //Изменяемый Drug не найден
            if (drug == null)
                throw new ApplicationException(string.Format("not found changes Drug Id = {0}", model.DrugId));

            if (model.DrugPackings.Count == 0)
                throw new ApplicationException("Удалены все упаковки");


            //Получаем описание всех параметров Drug
            var drugPropery = GetDrugPropery(model);

            //Ищем Drug по новым параметрам
            var drugAlreadyExists = FindDrug(drugPropery);

            //С тамким характеристиками уже есть другой Drug
            if (drugAlreadyExists != null && drugAlreadyExists.Id != model.DrugId)
            {
                throw new ApplicationException("Найден другой Drug с такими же характеристиками");
            }

            //Если что-то поменялось
            drug.ConsumerPackingCount = drugPropery.ConsumerPackingCount;
            drug.DosageGroup = drugPropery.DosageGroup;
            drug.FormProduct = drugPropery.FormProduct;
            drug.INNGroup = drugPropery.INNGroup;
            drug.TradeName = drugPropery.TradeName;
            drug.DrugTypeId = drugPropery.DrugTypeId;

            //Обновляем реальные упаковки
            var realPackingIdExists = _context.RealPacking.Where(r => r.DrugId == model.DrugId).ToList();

            foreach (var realPack in realPackingIdExists)
            {
                if (model.RealPackingList == null || model.RealPackingList.All(d => d.Id != realPack.Id))
                    _context.RealPacking.Remove(realPack);
            }

            if (model.RealPackingList != null)
            {
                foreach (var pack in model.RealPackingList.Where(r => r.Id == 0))
                {
                    _context.RealPacking.Add(new RealPacking()
                    {
                        DrugId = model.DrugId,
                        RealPackingCount = pack.RealPackingCount
                    });
                }
            }

            //Получаем информацию о производителе упаковщике
            var ownerTradeMark = FindManufacturer(model.OwnerTradeMark) ?? CreateManufacturer(model.OwnerTradeMark);
            var packer = FindManufacturer(model.Packer) ?? CreateManufacturer(model.Packer);

            var needReClassier = ownerTradeMark.Id != model.OwnerTradeMarkId || packer.Id != model.PackerId;

            var producitonInfo = _context.ProductionInfo.First(p => p.Id == model.ProductionInfoId);

            var existproductionInfo = FindProductionInfo(ownerTradeMark, packer, drug);

            if (existproductionInfo != null && producitonInfo.Id != existproductionInfo.Id)
                throw new ApplicationException("У выбранного Drug уже есть такие производитель и упаковщик");


            producitonInfo.OwnerTradeMark = ownerTradeMark;
            producitonInfo.Packer = packer;
            producitonInfo.Used = model.Used;

            //Обновляем инфорамцию об упаковках

            //Собираем DrugPacking

            var drugPackingNewList = new List<DrugPacking>();

            foreach (var drugPackingJson in model.DrugPackings)
            {
                DrugPacking drugPacking = null;

                if (drug.Id > 0)
                    drugPacking = FindDrugPacking(drug, drugPackingJson);


                if (drugPacking == null)
                {
                    drugPacking = CreateDrugPacking(drug, drugPackingJson);
                }

                if (drugPacking.ProductionInfo == null || drugPacking.ProductionInfo.All(p => p.Id != producitonInfo.Id))
                {
                    if (drugPacking.ProductionInfo == null)
                        drugPacking.ProductionInfo = new List<ProductionInfo>();

                    drugPacking.ProductionInfo.Add(producitonInfo);
                }

                drugPackingNewList.Add(drugPacking);
            }

            //Удаляем упаковки, которых нету.

            var drugPackingsExists = producitonInfo.DrugPacking.ToList();

            foreach (var drugPackingsExist in drugPackingsExists)
            {
                if (drugPackingNewList.All(p => p.Id != drugPackingsExist.Id))
                {
                    drugPackingsExist.ProductionInfo.Remove(producitonInfo);

                    if (drugPackingsExist.ProductionInfo.Count == 0) ;
                    DeleteDrugPacking(drugPackingsExist);
                }
            }

            if (drug.DrugPacking.Count == 0)
                throw new ApplicationException("Удалены все упаковки");

            if (needReClassier)
            {
                //Теперь переносим связанные данные
                ClassifierInfo from = new ClassifierInfo()
                {
                    DrugId = drug.Id,
                    OwnerTradeMarkId = model.OwnerTradeMarkId,
                    PackerId = model.PackerId
                };

                ClassifierInfo to = new ClassifierInfo()
                {
                    DrugId = drug.Id,
                    OwnerTradeMarkId = ownerTradeMark.Id,
                    PackerId = packer.Id
                };

                ReClassifier(from, to);
            }


            


            //Сохраняем изменения
            _context.SaveChanges();
            transaction.Commit();


      

            info.DrugId = drug.Id;
            info.PackerId = packer.Id;
            info.OwnerTradeMarkId = ownerTradeMark.Id;
            info.Used = producitonInfo.Used;

            return info;

        }


        public ClassifierInfoModel AddClassifier(ClassifierEditorModelJson model, bool tryMode)
        {
            model.Clear();

            InfoAboutNewClassifier addModel = new InfoAboutNewClassifier();

            var drugProperty = GetDrugPropery(model);

            if (drugProperty.TradeName.Id == 0)
                addModel.Description = string.Empty;

            //Ищем Drug
            Drug drug = null;

            // Ищем если
            if (!drugProperty.IsNew)
                drug = FindDrug(drugProperty);

            if (drug == null)
                drug = CreateDrug(drugProperty);



            //Собираем DrugPacking
            var drugPackingList = new List<DrugPacking>();

            foreach (var drugPackingJson in model.DrugPackings)
            {
                DrugPacking drugPacking = null;

                if (drug.Id > 0)
                    drugPacking = FindDrugPacking(drug, drugPackingJson);


                if (drugPacking == null)
                {
                    drugPacking = CreateDrugPacking(drug, drugPackingJson);
                }

                drugPackingList.Add(drugPacking);
            }

            //OnwerTradeMark

            Manufacturer ownerTradeMark = FindManufacturer(model.OwnerTradeMark)??CreateManufacturer(model.OwnerTradeMark);
            Manufacturer packer = FindManufacturer(model.Packer) ?? CreateManufacturer(model.Packer);

            RegistrationCertificate certificate = null;

            //Ищем сертификаты

            if (model.SelectedRegistrationCertificate == null && (ownerTradeMark.Key != "0" || packer.Key != "0") && drug.DrugTypeId == 1)
                throw new ApplicationException("не задан регистрационный сертификат");

            if (model.SelectedRegistrationCertificate != null)
            {
                certificate = FindCertificate(model.SelectedRegistrationCertificate) ?? CreateCertificate(model.SelectedRegistrationCertificate);

                //Только для новых сертификатов 
                if (certificate.Id == 0)
                {
                    Manufacturer ownerRegistrationCertificate = FindManufacturer(model.OwnerRegistrationCertificate) ?? CreateManufacturer(model.OwnerRegistrationCertificate);
                    certificate.OwnerRegistrationCertificate = ownerRegistrationCertificate;
                }
            }

            //Ищем ProductionInfo

            var productionInfo = FindProductionInfo(ownerTradeMark, packer, drug);

            if (productionInfo == null)
            {
                productionInfo = new ProductionInfo()
                 {
                     Drug = drug,
                     OwnerTradeMark = ownerTradeMark,
                     Packer = packer,
                     Used = model.Used,
                     RegistrationCertificate = new List<RegistrationCertificate>() { certificate },
                     DrugPacking = new List<DrugPacking>()
                 };

                _context.ProductionInfo.Add(productionInfo);
            }
            else
            {
                if (productionInfo.RegistrationCertificate.All(r => certificate != null && r.Id != certificate.Id))
                {
                    //Временно отключил
                    //if(productionInfo.RegistrationCertificate.Count > 0)
                    //    throw new ApplicationException("Нельзя добавлять более 1 связки LKCU, Owner, Ру");

                    productionInfo.RegistrationCertificate.Add(certificate);
                }
            }



            foreach (var drugPacking in drugPackingList)
            {
                productionInfo.DrugPacking.Add(drugPacking);
            }

            //Собираем информация о том, что,изменилось

            ClassifierInfoModel info = new ClassifierInfoModel { KCU = new List<string>() };

            info.LKCU = drug.LKCU.ToString();
            info.LKCUNew = drug.Id == 0;

            info.OwnerTradeMarkKeyNew = ownerTradeMark.Id == 0;

            if (info.OwnerTradeMarkKeyNew)
            {
                var ownerKeyOrder = _context.Manufacturer.Max(d => d.KeyOrder);
                ownerTradeMark.KeyOrder = ownerKeyOrder + 1;
                ownerTradeMark.Key = (ownerKeyOrder + 1).ToString();
                info.OwnerTradeMarkKey = ownerTradeMark.Key;
            }
            info.OwnerTradeMarkKey = ownerTradeMark.Key;


            info.PackerKeyNew = packer.Id == 0;

            if (packer.Id == 0)
            {
                var packerKeyOrder = _context.Manufacturer.Max(d => d.KeyOrder);
                packer.Key = (packerKeyOrder + 1).ToString();
                packer.KeyOrder = packerKeyOrder + 1;
                info.PackerKey = packer.Key;
            }

            info.PackerKey = packer.Key;

            info.OwnerRegistrationCertificateKeyNew = false;
            info.OwnerRegistrationCertificateKey = String.Empty;

            info.RegistrationCertificateNumber = String.Empty;
            info.RegistrationCertificateNumberNew = false;

            if (certificate != null)
            {

                info.OwnerRegistrationCertificateKeyNew = certificate.OwnerRegistrationCertificate != null &&
                                                          certificate.OwnerRegistrationCertificate.Id == 0;

                if (certificate.OwnerRegistrationCertificate != null && certificate.OwnerRegistrationCertificate.Id == 0)
                {
                    var ownerRegCertOrder = _context.Manufacturer.Max(d => d.KeyOrder);
                    certificate.OwnerRegistrationCertificate.Key = (ownerRegCertOrder + 1).ToString();
                    certificate.OwnerRegistrationCertificate.KeyOrder = ownerRegCertOrder + 1;
                    info.OwnerRegistrationCertificateKey = certificate.OwnerRegistrationCertificate.Key;
                }

                info.OwnerRegistrationCertificateKey = certificate.OwnerRegistrationCertificate != null
                    ? certificate.OwnerRegistrationCertificate.Key
                    : String.Empty;

                info.RegistrationCertificateNumber = certificate.Number;
                info.RegistrationCertificateNumberNew = certificate != null && certificate.Id == 0;

            }

            if (drugPackingList != null && drugPackingList.Any(d => d.Id == 0))
            {
                foreach (var drugPacking in drugPackingList.Where(d => d.Id == 0))
                {
                    info.KCU.Add(drugPacking.KCU);
                }
            }

            //RealPacking

            AddRealPacking(model.RealPackingList, drug);

            //Save

            if (!tryMode)
            {
                _context.SaveChanges();


                info.DrugId = drug.Id;
                info.OwnerTradeMarkId = ownerTradeMark.Id;
                info.PackerId = packer.Id;
                info.Used = model.Used;
            }

            
            info.Drug = drug;
            info.ProductionInfo = productionInfo;

            //Ищем информацию о сертификате
            return info;
        }

        #endregion Action

        #region Drug

        private class DrugProperty
        {
            public TradeName TradeName { get; set; }
            public FormProduct FormProduct { get; set; }
            public INNGroup INNGroup { get; set; }
            public DosageGroup DosageGroup { get; set; }
            public bool IsNew { get; set; }
            public int? ConsumerPackingCount { get; set; }
            public long DrugTypeId { get; set; }
        }

        //Получить все характеристики для Drug
        private DrugProperty GetDrugPropery(ClassifierEditorModelJson model)
        {
            //Ищем tradeName
            var tradeName = FindOrCreateTradeName(model);

            if (tradeName == null)
                throw new ApplicationException("ТН не может быть пустым");

            var formProduct = FindOrCreateFormProduct(model);

            if (formProduct == null)
                throw new ApplicationException("форма выпуска должна быть указана");

            //Ищем innGroup
            var innGroup = FindOrCreateInnGroup(model);

            //Начинаем искать
            DosageGroup dosageGroup = FindDosageGroup(model) ?? CreateDosageGroup(model);

            //Прямые признаки того что Drug будет новым
            bool isNew = tradeName.Id == 0 || (innGroup != null && innGroup.Id == 0) || formProduct.Id == 0 || (dosageGroup != null && dosageGroup.Id == 0);

            var drugPropery = new DrugProperty
            {
                TradeName = tradeName,
                FormProduct = formProduct,
                INNGroup = innGroup,
                DosageGroup = dosageGroup,
                ConsumerPackingCount = model.ConsumerPackingCount,
                DrugTypeId = model.DrugType.Id,
                IsNew = isNew
            };

            return drugPropery;
        }

        //Получаем существующий или новый Drug
        private Drug GetDrug(ClassifierEditorModelJson model)
        {
            var drugProperty = GetDrugPropery(model);

            ///Ищем Drug
            Drug drug = null;

            // Ищем если
            if (drugProperty.IsNew)
                drug = CreateDrug(drugProperty);
            else
                drug = FindDrug(drugProperty);


            if (drug == null)
                drug = CreateDrug(drugProperty);

            //Собираем DrugPacking
            foreach (var drugPackingJson in model.DrugPackings)
            {
                DrugPacking drugPacking = null;

                if (drug.Id > 0)
                    drugPacking = FindDrugPacking(drug, drugPackingJson);


                if (drugPacking == null)
                {
                    CreateDrugPacking(drug, drugPackingJson);
                }
            }

            return drug;
        }

        private Drug CheckDrug(ClassifierEditorModelJson model)
        {
            var drugProperty = GetDrugPropery(model);

            if (drugProperty.IsNew)
                return null;

            var drug = FindDrug(drugProperty);

            return drug;
        }

        private Drug CreateDrug(DrugProperty property)
        {

            Drug drug = new Drug();
            drug.FormProduct = property.FormProduct;
            drug.DosageGroup = property.DosageGroup;
            drug.DrugTypeId = property.DrugTypeId;
            drug.ConsumerPackingCount = property.ConsumerPackingCount;
            drug.TradeName = property.TradeName;
            drug.INNGroup = property.INNGroup;
            drug.LKCU = GetNextLKCU();

            drug.DrugPacking = new List<DrugPacking>();

            _context.Drugs.Add(drug);

            return drug;
        }

        private long GetNextLKCU()
        {
            var lkcu = _context.Drugs.Max(d => d.LKCU);

            return lkcu + 1 ?? 1
        }

        private Drug FindDrug(DrugProperty property)
        {
            var drugs = _context.Drugs.Where(d => d.FormProductId == property.FormProduct.Id &&
                                                    d.TradeNameId == property.TradeName.Id &&
                                                    d.DrugTypeId == property.DrugTypeId);

            if (property.INNGroup != null)
                drugs = drugs.Where(d => d.INNGroupId == property.INNGroup.Id);
            else
                drugs = drugs.Where(d => d.INNGroupId == null);

            if (property.ConsumerPackingCount.HasValue)
                drugs = drugs.Where(d => d.ConsumerPackingCount.HasValue && d.ConsumerPackingCount.Value == property.ConsumerPackingCount.Value);
            else
                drugs = drugs.Where(d => !d.ConsumerPackingCount.HasValue);

            if (property.DosageGroup != null)
                drugs = drugs.Where(d => d.DosageGroupId.HasValue && d.DosageGroupId.Value == property.DosageGroup.Id);
            else
                drugs = drugs.Where(d => !d.DosageGroupId.HasValue);

            var foundDrugs = drugs.ToList();

            if (foundDrugs.Count > 1)
                throw new ApplicationException("нарушение целостности Drug");

            if (foundDrugs.Count == 1)
                return foundDrugs.First();

            return null;
        }

        #endregion Drug

        #region DeleteOperation


        private void DeleteDrugPacking(DrugPacking drugPacking)
        {
            drugPacking.ProductionInfo = null;
            drugPacking.Drug = null;
            _context.DrugPackings.Remove(drugPacking);
        }


        private void DeleteProductionInfo(Drug drug, Manufacturer ownerTradeMark, Manufacturer packer)
        {
            var existsproductionInfo = FindProductionInfo(ownerTradeMark, packer, drug);

            if (existsproductionInfo == null)
                return;

            existsproductionInfo.RegistrationCertificate.ToList().ForEach(rc => rc.ProductionInfo.Remove(existsproductionInfo));

            existsproductionInfo.RegistrationCertificate = null;
            existsproductionInfo.Drug = null;
            existsproductionInfo.DrugPacking.ToList().ForEach(dp => dp.ProductionInfo.Remove(existsproductionInfo));
            existsproductionInfo.DrugPacking = null;
            _context.ProductionInfo.Remove(existsproductionInfo);
        }

   

        private void DeleteDrug(Drug drug)
        {
            foreach (var drugPacking in drug.DrugPacking)
            {
                DeleteDrugPacking(drugPacking);
            }
            _context.Drugs.Remove(drug);
        }


        #endregion DeleteOperation

        #region DrugProperty


        private List<INNDosage> CreateDosageList(ClassifierEditorModelJson model)
        {
            var dosageList = new List<INNDosage>();

            if (model.InnGroupDosage != null && model.InnGroupDosage.Any())
            {
                for (var i = 0; i < model.InnGroupDosage.Count; i++)
                {
                    var modelDosage = model.InnGroupDosage[i];

                    var dosage = FindOrCreateDosage(modelDosage.Dosage);


                    dosageList.Add(new INNDosage()
                    {
                        Dosage = dosage,
                        DosageCount = model.InnGroupDosage[i].DosageCount,
                        Order = i + 1
                    });
                }
            }
            return dosageList;
        }

        private INNGroup FindOrCreateInnGroup(ClassifierEditorModelJson model)
        {
            INNGroup innGroup = null;

            if (model.InnGroupDosage != null && model.InnGroupDosage.Any())
            {
                var innList = new List<INN>();

                foreach (var innModel in model.InnGroupDosage)
                {
                    var inn = FindOrCreateInn(innModel.INN);

                    innList.Add(inn);
                }

                if (innList.All(i => i.Id > 0))
                {
                    innGroup = FindInnGroup(innList);
                }

                if (innGroup == null)
                {
                    innGroup = CreateInnGroup(innList, model.InnGroupDosageDescription);

                }
            }
            return innGroup;
        }

        private DrugPacking CreateDrugPacking(Drug drug, DrugPackingJson drugPackingJson)
        {

            Packing consumerPacking = FindOrCreatePacking(drugPackingJson.ConsumerPacking);
            Packing primaryPacking = FindOrCreatePacking(drugPackingJson.PrimaryPacking);


            DrugPacking drugPacking = new DrugPacking()
            {
                ConsumerPacking = consumerPacking,
                PrimaryPacking = primaryPacking,
                CountInPrimaryPacking = drugPackingJson.CountInPrimaryPacking,
                CountPrimaryPacking = drugPackingJson.CountPrimaryPacking,
                PackingDescription = drugPackingJson.PackingDescription,
                Drug = drug
            };

            var kcuOrder = drug.DrugPacking.Max(d => d.KCUOrder);

            if (!kcuOrder.HasValue)
            {
                drugPacking.KCU = string.Format("{0}/{1}", drug.LKCU, 1);
                drugPacking.KCUOrder = 1;
            }
            else
            {
                drugPacking.KCU = string.Format("{0}/{1}", drug.LKCU, kcuOrder + 1);
                drugPacking.KCUOrder = kcuOrder + 1;
            }        

            _context.DrugPackings.Add(drugPacking);

            return drugPacking;
        }

        private DrugPacking FindDrugPacking(Drug drug, DrugPackingJson drugPackingJson)
        {

            Packing consumerPacking = FindOrCreatePacking(drugPackingJson.ConsumerPacking);
            Packing primaryPacking = FindOrCreatePacking(drugPackingJson.PrimaryPacking);

            if (consumerPacking != null && consumerPacking.Id == 0)
                return null;
            
            if (primaryPacking != null && primaryPacking.Id == 0)
                return null;

            var drugPacking = _context.DrugPackings.Where(d => d.DrugId == drug.Id);

            if (consumerPacking != null && consumerPacking.Id > 0)
            {
                drugPacking = drugPacking.Where(d => d.ConsumerPackingId == consumerPacking.Id);
            }

            if (consumerPacking == null)
                drugPacking = drugPacking.Where(d => !d.ConsumerPackingId.HasValue);


            if (primaryPacking != null && primaryPacking.Id > 0)
            {
                drugPacking = drugPacking.Where(d => d.PrimaryPackingId == primaryPacking.Id);
            }

            if (primaryPacking == null)
                drugPacking = drugPacking.Where(d => !d.PrimaryPackingId.HasValue);


            if (string.IsNullOrEmpty(drugPackingJson.PackingDescription))
            {
                drugPacking = drugPacking.Where(d => d.PackingDescription == null);
            }
            else
            {
                drugPacking = drugPacking.Where(d => string.Equals(d.PackingDescription, drugPackingJson.PackingDescription));
            }


            var drugPackingFound = drugPacking.Where(d => d.CountInPrimaryPacking == drugPackingJson.CountInPrimaryPacking && d.CountPrimaryPacking == drugPackingJson.CountPrimaryPacking).ToList();

            if (drugPackingFound.Count == 0)
            {
               var drugPackingFoundLocal = FindDrugPackingLocal(drug, drugPackingJson);

                if (drugPackingFoundLocal != null)
                    return drugPackingFoundLocal;
            }


            if (drugPackingFound.Count > 1)
                throw new ApplicationException("Найдено более двух одинаковых KCU");

            if (drugPackingFound.Count == 1)
                return drugPackingFound.First();

            return null;


        }

        private DrugPacking FindDrugPackingLocal(Drug drug, DrugPackingJson drugPackingJson)
        {

            Packing consumerPacking = FindOrCreatePacking(drugPackingJson.ConsumerPacking);
            Packing primaryPacking = FindOrCreatePacking(drugPackingJson.PrimaryPacking);

            if (consumerPacking != null && consumerPacking.Id == 0)
                return null;


            if (primaryPacking != null && primaryPacking.Id == 0)
                return null;

            var drugPacking = _context.DrugPackings.Local.Where(d => d.DrugId == drug.Id);

            if (consumerPacking != null && consumerPacking.Id > 0)
            {
                drugPacking = drugPacking.Where(d => d.ConsumerPackingId == consumerPacking.Id);
            }

            if (consumerPacking == null)
                drugPacking = drugPacking.Where(d => !d.ConsumerPackingId.HasValue);


            if (primaryPacking != null && primaryPacking.Id > 0)
            {
                drugPacking = drugPacking.Where(d => d.PrimaryPackingId == primaryPacking.Id);
            }

            if (primaryPacking == null)
                drugPacking = drugPacking.Where(d => !d.PrimaryPackingId.HasValue);


            if (string.IsNullOrEmpty(drugPackingJson.PackingDescription))
            {
                drugPacking = drugPacking.Where(d => d.PackingDescription == null);
            }
            else
            {
                drugPacking = drugPacking.Where(d => string.Equals(d.PackingDescription, drugPackingJson.PackingDescription));
            }


            var drugPackingFound = drugPacking.Where(d => d.CountInPrimaryPacking == drugPackingJson.CountInPrimaryPacking && d.CountPrimaryPacking == drugPackingJson.CountPrimaryPacking).ToList();


            if (drugPackingFound.Count > 1)
                throw new ApplicationException("Найдено более двух одинаковых KCU");

            if (drugPackingFound.Count == 1)
                return drugPackingFound.First();

            return null;


        }

        private Packing FindPacking(DictionaryJson packingJson)
        {
            if (packingJson == null)
                return null;

            if (packingJson.Id > 0)
                return _context.Packings.First(p => p.Id == packingJson.Id);

            if (!string.IsNullOrEmpty(packingJson.Value))
            {
                var foundPackings = _context.Packings.Where(p => p.Value == packingJson.Value).ToList();

                if (foundPackings.Count == 0)
                    foundPackings = _context.Packings.Local.Where(p => p.Value == packingJson.Value).ToList();

                if (foundPackings.Count > 1)
                    throw new ApplicationException("Нарушение целостности Packing");

                if (foundPackings.Count == 1)
                    return foundPackings.First();
            }

            return null;
        }

        private Packing FindOrCreatePacking(DictionaryJson packingJson)
        {
            if (packingJson == null)
                return null;

            Packing packing = FindPacking(packingJson);


            if (packing == null && !string.IsNullOrEmpty(packingJson.Value))
            {
                packing = new Packing()
                {
                    Value = packingJson.Value
                };

                _context.Packings.Add(packing);

                return packing;
            }

            return packing;
        }

        private void AddRealPacking(List<RealPackingCountJson> realPackingList, Drug drug)
        {
            if (realPackingList != null)
            {

                foreach (var pack in realPackingList)
                {
                    _context.RealPacking.Add(new RealPacking()
                    {
                        Drug = drug,
                        RealPackingCount = pack.RealPackingCount
                    });
                }
            }
        }
        
        private INNGroup CreateInnGroup(List<INN> innList, string description)
        {
            var innGroup = new INNGroup { Description = description, INNGroup_INN = new List<INNGroup_INN>() };

            for (var i = 0; i < innList.Count; i++)
            {
                innGroup.INNGroup_INN.Add(new INNGroup_INN() { INN = innList[i], Order = i + 1 });
            }

            _context.INNGroups.Add(innGroup);

            return innGroup;
        }

        /// <summary>
        /// Список INN c учетом порядка
        /// </summary>
        /// <param name="innList"></param>
        /// <returns></returns>
        private INNGroup FindInnGroup(List<INN> innList)
        {

            var findInnGroups = new List<long>();


            for (var i = 0; i < innList.Count; i++)
            {
                var innId = innList[i].Id;
                var order = i + 1;
                var inns = _context.INNGroup_INN.Where(g => g.Order == order && g.INNId == innId);

                if (findInnGroups.Count == 0)
                    findInnGroups = inns.GroupBy(n => n.INNGroupId).Select(g => g.Key).ToList();
                else
                {
                    findInnGroups = findInnGroups.Intersect(inns.GroupBy(n => n.INNGroupId).Select(g => g.Key).ToList()).ToList();
                }
            }

            if (findInnGroups.Count > 0)
            {
                findInnGroups = findInnGroups.Intersect(
                    _context.INNGroup_INN.Where(i => findInnGroups.Contains(i.INNGroupId))
                        .GroupBy(n => n.INNGroupId)
                        .Where(g => g.Count() == innList.Count)
                        .Select(g => g.Key)
                        .ToList()).ToList();
            }
            else
            {
                return null;
            }

            if (findInnGroups.Count() == 1)
            {
                var innGroupId = findInnGroups.First();
                return _context.INNGroups.First(g => g.Id == innGroupId);
            }

            if (findInnGroups.Count() > 1)
            {
                //Временно отключил
                //    throw new ApplicationException("ups many innGroups found");
                var innGroupId = findInnGroups.First();
                return _context.INNGroups.First(g => g.Id == innGroupId);
            }
              

            return null;

        }

        private INN FindInn(DictionaryJson innModel)
        {
            INN inn = null;
            if (innModel.Id > 0)
            {
                inn = _context.INNs.FirstOrDefault(i => i.Id == innModel.Id);
            }
            else if (!string.IsNullOrEmpty(innModel.Value))
            {
                var innsFound = _context.INNs.Where(i => i.Value == innModel.Value).ToList();
                if (innsFound.Count == 0)
                    innsFound = _context.INNs.Local.Where(i => i.Value == innModel.Value).ToList();

                if (innsFound.Count > 1)
                    throw new ApplicationException("нарушение целостности по inn");
                if (innsFound.Count == 1)
                    inn = innsFound.First();

            }

            return inn;

        }

        private INN FindOrCreateInn(DictionaryJson innModel)
        {
            INN inn = FindInn(innModel);

            if (inn == null && !string.IsNullOrEmpty(innModel.Value))
            {

                inn = new INN { Value = innModel.Value };
                _context.INNs.Add(inn);
            }

            return inn;

        }

        /// <summary>
        /// Форма выпуска
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private FormProduct FindFormProduct(ClassifierEditorModelJson model)
        {
            FormProduct formProduct = null;

            if (model.FormProduct.Id > 0)
            {
                formProduct = _context.FormProducts.First(tn => tn.Id == model.FormProduct.Id);
            }
            else if (!string.IsNullOrEmpty(model.TradeName.Value))
            {
                var formProducts = _context.FormProducts.Where(tn => tn.Value == model.FormProduct.Value).ToList();

                if (formProducts.Count > 1)
                    throw new ApplicationException("нарушение целостности формы выпуска");

                if (formProducts.Count == 1)
                    return formProducts.First();
            }

            return formProduct;
        }

        private FormProduct FindOrCreateFormProduct(ClassifierEditorModelJson model)
        {
            FormProduct formProduct = FindFormProduct(model);

            if (formProduct == null && !string.IsNullOrEmpty(model.FormProduct.Value))
            {
                formProduct = new FormProduct { Value = model.FormProduct.Value };
                _context.FormProducts.Add(formProduct);
            }

            return formProduct;
        }

        private TradeName FindTradeName(ClassifierEditorModelJson model)
        {
            TradeName tradeName = null;

            if (model.TradeName.Id > 0)
            {
                tradeName = _context.TradeNames.First(tn => tn.Id == model.TradeName.Id);
            }
            else if (!string.IsNullOrEmpty(model.TradeName.Value))
            {
                var tradeNames = _context.TradeNames.Where(tn => tn.Value == model.TradeName.Value).ToList();

                if (tradeNames.Count > 1)
                    throw new ApplicationException("нарушение целостности формы выпуска");

                if (tradeNames.Count == 1)
                    return tradeNames.First();
            }

            return tradeName;
        }
        
        private TradeName FindOrCreateTradeName(ClassifierEditorModelJson model)
        {
            TradeName tradeName = FindTradeName(model);


            if (tradeName == null && !string.IsNullOrEmpty(model.TradeName.Value))
            {
                tradeName = new TradeName() { Value = model.TradeName.Value };
                _context.TradeNames.Add(tradeName);
            }

            return tradeName;

        }

        private DosageGroup FindDosageGroup(ClassifierEditorModelJson model)
        {

            var dosageList = CreateDosageList(model);

            Dosage dosageValue = null;

            if (model.DosageValue != null)
            {
                dosageValue = FindDosage(model.DosageValue);
            }

            if (dosageValue != null && dosageValue.Id == 0)
                return null;

            Dosage dosageTotal = null;

            if (model.TotalVolume != null)
            {
                dosageTotal = FindDosage(model.TotalVolume);
            }

            if (dosageTotal != null && dosageTotal.Id == 0)
                return null;

            var dosageGroups = _context.DosageGroups.Where(d => d != null);

            if (dosageValue != null)
                dosageGroups = dosageGroups.Where(d => d.DosageValueId == dosageValue.Id);
            else
                dosageGroups = dosageGroups.Where(d => d.DosageValueId == null);

            if (dosageTotal != null)
                dosageGroups = dosageGroups.Where(d => d.TotalVolume.Id == dosageTotal.Id);
            else
                dosageGroups = dosageGroups.Where(d => d.TotalVolume == null);


            var dosageGroupsFound = dosageGroups.Where(d => d.DosageValueCount == model.DosageValueCount && d.TotalVolumeCount == model.TotalVolumeCount).ToList();

            if (dosageGroupsFound.Count == 0)
                return null;


            for (int i = 0; i < dosageList.Count; i++)
            {
                var dosage = dosageList[i];

                List<INNDosage> inndosagesList = null;

                if (dosage.Dosage == null && string.IsNullOrEmpty(dosage.DosageCount))
                    continue;

                if (dosage.Order != null)
                {
                    var dosageId = dosage.Dosage != null ? dosage.Dosage.Id : (long?)null;


                    inndosagesList = _context.INNDosage.Where(inndosage => inndosage.Order == dosage.Order &&
                                                                           inndosage.DosageId == dosageId &&
                                                                           inndosage.DosageCount == dosage.DosageCount).ToList();
                }


                else
                {
                    inndosagesList = _context.INNDosage.Where(inndosage => inndosage.Order == dosage.Order &&
                                                                           inndosage.DosageId == null &&
                                                                           inndosage.DosageCount == dosage.DosageCount)
                        .ToList();
                }



                var dosageInnGroups = inndosagesList.Select(idl => idl.DosageGroup).ToList();

                dosageGroupsFound = dosageGroupsFound.Intersect(dosageInnGroups).ToList();



                if (dosageGroupsFound.Count == 0)
                    return null;
            }


            var dosageInnCount = dosageList.Count(c => c.Dosage != null);

            dosageGroupsFound = dosageGroupsFound.Where(d => d.INNDosages != null && d.INNDosages.Count == dosageInnCount).ToList();


            if (dosageGroupsFound.Count > 1)
                throw new ApplicationException("нарушение целостности группировки дозировок");

            if (dosageGroupsFound.Count == 1)
                return dosageGroupsFound.First();

            return null;

        }

        private DosageGroup CreateDosageGroup(ClassifierEditorModelJson model)
        {
            var dosageList = CreateDosageList(model);

            Dosage dosageValue = null;

            if (model.DosageValue != null)
            {
                dosageValue = FindOrCreateDosage(model.DosageValue);
            }

            Dosage dosageTotal = null;

            if (model.TotalVolume != null)
            {
                dosageTotal = FindOrCreateDosage(model.TotalVolume);
            }

            if (dosageList.Count(d => d.Dosage != null || d.DosageCount != null) == 0 &&
                dosageTotal == null && dosageValue == null &&
                model.DosageValueCount == null &&
                model.TotalVolumeCount == null)
                return null;

            var dosageGroup = new DosageGroup
            {
                DosageValue = dosageValue,
                DosageValueCount = model.DosageValueCount,
                TotalVolume = dosageTotal,
                TotalVolumeCount = model.TotalVolumeCount,
                Description = model.DosageGroupDescription,
                INNDosages = new List<INNDosage>()
            };

            foreach (var dosage in dosageList.Where(d => d.Dosage != null || d.DosageCount != null))
            {
                dosageGroup.INNDosages.Add(new INNDosage()
                {
                    Dosage = dosage.Dosage,
                    DosageCount = dosage.DosageCount,
                    Order = dosage.Order
                });
            }

            _context.DosageGroups.Add(dosageGroup);
            return dosageGroup;
        }

        private Dosage FindDosage(DictionaryJson dosageJson)
        {
            if (dosageJson == null)
                return null;

            if (dosageJson.Id > 0)
                return _context.Dosages.First(d => d.Id == dosageJson.Id);

            if (!string.IsNullOrEmpty(dosageJson.Value))
            {
                var dosagesValue = _context.Dosages.Where(d => d.Value == dosageJson.Value).ToList();
                if (dosagesValue.Count == 0)
                    dosagesValue = _context.Dosages.Local.Where(d => d.Value == dosageJson.Value).ToList();

                if (dosagesValue.Count() > 1)
                    throw new ApplicationException("Нарушение целостности дозировок");

                if (dosagesValue.Count() == 1)
                    return dosagesValue.First();

            }

            return null;
        }

        private Dosage FindOrCreateDosage(DictionaryJson dosageJson)
        {
            var dosage = FindDosage(dosageJson);

            if (dosage == null && dosageJson != null && !string.IsNullOrEmpty(dosageJson.Value))
            {

                dosage = new Dosage() { Value = dosageJson.Value };
                _context.Dosages.Add(dosage);
                return dosage;

            }

            return dosage;
        }
        
        #endregion DrugProperty
        
        #region RegistrationCertificate

        private RegistrationCertificate FindCertificate(RegistrationCertificateJson registrationCeritifacte)
        {

            if (registrationCeritifacte == null)
                return null;

            var circulationPeriod = FindCirculationPeriod(registrationCeritifacte.CirculationPeriod);

            if (circulationPeriod == null && registrationCeritifacte.CirculationPeriod != null &&
                !string.IsNullOrEmpty(registrationCeritifacte.CirculationPeriod.Value)) return null;



            long? idPeriod = null;

            if (circulationPeriod != null)
                idPeriod = circulationPeriod.Id;

            var regCertFound =
                _context.RegistrationCertificates.Where(rc => rc.Number == registrationCeritifacte.Number &&
                                                              rc.Url == registrationCeritifacte.url &&
                                                              rc.CirculationPeriodId == idPeriod);

            if (registrationCeritifacte.RegistrationDate.HasValue)
            {

                var date = registrationCeritifacte.RegistrationDate.Value.Date;

                regCertFound =
                    regCertFound.Where(
                        rc =>
                            rc.RegistrationDate.HasValue &&
                            rc.RegistrationDate.Value.Year == date.Year &&
                            rc.RegistrationDate.Value.Month == date.Month &&
                            rc.RegistrationDate.Value.Day == date.Day);
            }

            else
                regCertFound = regCertFound.Where(rc => !rc.RegistrationDate.HasValue);


            if (registrationCeritifacte.ExpDate.HasValue)
            {
                var date = registrationCeritifacte.ExpDate.Value.Date;

                regCertFound = regCertFound.Where(rc => rc.ExpDate.HasValue &&
                            rc.ExpDate.Value.Year == date.Year &&
                            rc.ExpDate.Value.Month == date.Month &&
                            rc.ExpDate.Value.Day == date.Day);
            }
            else
                regCertFound = regCertFound.Where(rc => !rc.ExpDate.HasValue);


            if (registrationCeritifacte.ReissueDate.HasValue)
            {
                var date = registrationCeritifacte.ReissueDate.Value.Date;

                regCertFound = regCertFound.Where(rc => rc.ReissueDate.HasValue &&
                                                        rc.ReissueDate.Value.Year == date.Year &&
                                                        rc.ReissueDate.Value.Month == date.Month &&
                                                        rc.ReissueDate.Value.Day == date.Day);
            }
            else
                regCertFound = regCertFound.Where(rc => !rc.ReissueDate.HasValue);

            var regCertFoundList = regCertFound.ToList();


            if (regCertFoundList.Count > 1)
                throw new ApplicationException("Нарушение целостности сертификатов");

            if (regCertFoundList.Count == 1)
                return regCertFound.First();

            return null;
        }

        private CirculationPeriod FindCirculationPeriod(DictionaryJson periodJson)
        {
            CirculationPeriod period = null;

            if (periodJson == null)
                return null;

            if (periodJson.Id > 0)
            {
                period = _context.CirculationPeriod.First(tn => tn.Id == periodJson.Id);
            }
            else if (!string.IsNullOrEmpty(periodJson.Value))
            {
                var periods = _context.CirculationPeriod.Where(tn => tn.Value == periodJson.Value).ToList();

                if (periods.Count > 1)
                    throw new ApplicationException("нарушение целостности формы выпуска");

                if (periods.Count == 1)
                    return periods.First();
            }

            return period;
        }

        private CirculationPeriod FindOrCreateCirculationPeriod(DictionaryJson periodJson)
        {
            CirculationPeriod period = FindCirculationPeriod(periodJson);

            if (period == null && periodJson != null && !string.IsNullOrEmpty(periodJson.Value))
            {
                period = new CirculationPeriod { Value = periodJson.Value };
                _context.CirculationPeriod.Add(period);
            }

            return period;
        }
        
        private RegistrationCertificate CreateCertificate(RegistrationCertificateJson registrationCeritifacte)
        {
            if (string.IsNullOrEmpty(registrationCeritifacte.Number))
                return null;

            var circulationPeriod = FindOrCreateCirculationPeriod(registrationCeritifacte.CirculationPeriod);

            var regCert = new RegistrationCertificate()
            {
                Number = registrationCeritifacte.Number,
                Url = registrationCeritifacte.url,
                CirculationPeriod = circulationPeriod,
                RegistrationDate = registrationCeritifacte.RegistrationDate.HasValue ? registrationCeritifacte.RegistrationDate.Value.Date : (DateTime?)null,
                ExpDate = registrationCeritifacte.ExpDate.HasValue ? registrationCeritifacte.ExpDate.Value.Date : (DateTime?)null,
                ReissueDate = registrationCeritifacte.ReissueDate.HasValue ? registrationCeritifacte.ReissueDate.Value.Date : (DateTime?)null

            };

            _context.RegistrationCertificates.Add(regCert);


            return regCert;
        }

        #endregion RegistrationCertificate

        #region ProductionInfo

        private ProductionInfo FindProductionInfo(Manufacturer ownerTradeMark, Manufacturer packer, Drug drug)
        {
            ProductionInfo productionInfo = null;

            if (drug.Id > 0 && ownerTradeMark.Id > 0 && packer.Id > 0)
            {
                var productionInfoFound = _context.ProductionInfo.Where(p => p.OwnerTradeMarkId == ownerTradeMark.Id && p.PackerId == packer.Id && p.DrugId == drug.Id).ToList();

                if (productionInfoFound.Count > 1)
                    throw new ApplicationException("Нарушение целостности ProductionInfo");

                if (productionInfoFound.Count == 1)
                    productionInfo = productionInfoFound.First();
            }


            return productionInfo;
        }
        
        private ProductionInfo CreateProductionInfo(ClassifierEditorModelJson model, Drug drug, Manufacturer ownerTradeMark, Manufacturer packer)
        {
            var productionInfo = new ProductionInfo()
            {
                Drug = drug,
                OwnerTradeMark = ownerTradeMark,
                Packer = packer,
                Used = model.Used,
                DrugPacking = new List<DrugPacking>(),
                RegistrationCertificate = new List<RegistrationCertificate>()
            };
            _context.ProductionInfo.Add(productionInfo);

            return productionInfo;
        }

        private Manufacturer FindManufacturer(DictionaryJson manufacturerJson)
        {

            if (manufacturerJson.Id > 0)
                return _context.Manufacturer.First(p => p.Id == manufacturerJson.Id);           
        


            if (!string.IsNullOrEmpty(manufacturerJson.Value))
            {
                var foundManufacturers = _context.Manufacturer.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count == 0)
                    foundManufacturers = _context.Manufacturer.Local.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count > 1)
                    throw new ApplicationException("Нарушение целостности ownerTradeMark");

                if (foundManufacturers.Count == 1)
                    return foundManufacturers.First();

            }



            return null;
        }

        private Manufacturer CreateManufacturer(DictionaryJson manufacturerJson)
        {
            var manufacturer = FindManufacturer(manufacturerJson);

            if (manufacturer == null && !string.IsNullOrEmpty(manufacturerJson.Value))
            {

                var keyOrder = _context.Manufacturer.Max(k => k.KeyOrder);
                var localKeyOrder = _context.Manufacturer.Local.ToList().Max(k => k.KeyOrder);

                long maxOrder = 1;

                if(keyOrder.HasValue)
                    maxOrder = keyOrder.Value;

                if (localKeyOrder.HasValue)
                    maxOrder = localKeyOrder.Value;

                if (keyOrder.HasValue && localKeyOrder.HasValue)
                    maxOrder = keyOrder.Value > localKeyOrder.Value ? keyOrder.Value : localKeyOrder.Value;

                manufacturer = new Manufacturer()
                {
                    Value = manufacturerJson.Value,
                    Key = (maxOrder + 1).ToString(),
                    KeyOrder = maxOrder + 1
                };

                _context.Manufacturer.Add(manufacturer);
            }

            return manufacturer;
        }

        #endregion ProductionInfo
        
        private class ClassifierInfo
        {
            public long DrugId { get; set; }
            public long OwnerTradeMarkId { get; set; }
            public long PackerId { get; set; }
        }

        private void ReClassifier(ClassifierInfo from, ClassifierInfo to)
        {

            string upd = @" 
            UPDATE Systematization.DrugClassifier with(TABLOCKX)
            SET		DrugId = @ToDrugId, 
                    OwnerTradeMarkId = @ToOwnerTradeMarkId, 
                    PackerId = @ToPackerId
            WHERE	DrugId = @DrugId and 
                    OwnerTradeMarkId = @OwnerTradeMarkId and 
                    PackerId = @PackerId
	
            

            UPDATE Systematization.DrugClassifierInWork with(TABLOCKX)
            SET		DrugId = @ToDrugId, 
                    OwnerTradeMarkId = @ToOwnerTradeMarkId, 
                    PackerId = @ToPackerId
            WHERE	DrugId = @DrugId and 
                    OwnerTradeMarkId = @OwnerTradeMarkId and 
                    PackerId = @PackerId";

            var param = new object[]
            {
                new SqlParameter("ToDrugId", to.DrugId),
                new SqlParameter("ToOwnerTradeMarkId", to.OwnerTradeMarkId),
                new SqlParameter("ToPackerId", to.PackerId),
                new SqlParameter("DrugId", from.DrugId),
                new SqlParameter("OwnerTradeMarkId", from.OwnerTradeMarkId),
                new SqlParameter("PackerId", from.PackerId)
            };

            _context.Database.ExecuteSqlCommand(upd, param);

            
        }




        private string GetDesription(DrugPackingJson packing)
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(packing.PrimaryPacking.Value))
            {
                builder.Append(packing.PrimaryPacking.Value);
                builder.Append(" ");
            }

            if (!string.IsNullOrEmpty(packing.ConsumerPacking.Value))
            {
                builder.Append(packing.ConsumerPacking.Value);
                builder.Append(" ");
            }

            builder.Append(packing.CountPrimaryPacking);
            builder.Append(" ");

            builder.Append(packing.CountInPrimaryPacking);
            builder.Append(" ");

            builder.Append(packing.PackingDescription);
            
            return builder.ToString().Replace("  "," ").Trim();
        }

        private string GetDesription(DrugPacking packing)
        {
            StringBuilder builder = new StringBuilder();
            if (packing.PrimaryPacking != null)
            {
                builder.Append(packing.PrimaryPacking.Value);
                builder.Append(" ");
            }

            if (packing.ConsumerPacking != null)
            {
                builder.Append(packing.ConsumerPacking.Value);
                builder.Append(" ");
            }
           
                builder.Append(packing.CountPrimaryPacking);
                builder.Append(" ");
           
                builder.Append(packing.CountInPrimaryPacking);
                builder.Append(" ");
            
            builder.Append(packing.PackingDescription);

            return builder.ToString().Replace("  ", " ").Trim();
        }

    }


    public class InfoAboutNewClassifier
    {
        public string Description { get; set; }
    }
}
