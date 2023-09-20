using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebGrease.Css.Extensions;

namespace DataAggregator.Core.Classifier
{
    public class ClassifierEditor
    {
        private readonly DrugClassifierContext _context;

        private readonly Guid _user;

        private readonly ClassifierDictionary _dictionary;

        /// <summary>
        /// Добавление блистеровки
        /// Для всех типов перенести коэффициент в блок «блистеровка» автоматически из классификатора (поле количество первичных упаковок)
        /// если упаковка одна или одинаковое кол-во
        /// </summary>
        /// <param name="classifierInfo">новый классификатор</param>
        /// <param name="context">БД</param>
        private static void AddBlisterBlock(ClassifierInfo classifierInfo, DrugClassifierContext context)
        {
            if (classifierInfo == null)
            {
                throw new ArgumentException("Parameter cannot be null", nameof(classifierInfo));
            }

            long ClassifierId = classifierInfo.Id;

            #region находим Id упаковки: [Classifier].[ClassifierPacking].Id
            int? ClassifierPackingId = null;

            if (context.ClassifierPacking.Where(w => w.ClassifierId == ClassifierId).GroupBy(t => t.CountPrimaryPacking).Count() == 1)
            {
                ClassifierPackingId = context.ClassifierPacking.Where(w => w.ClassifierId == ClassifierId).FirstOrDefault().Id;
            }
            #endregion

            if (context.BlisterBlock.Find(ClassifierId) == null)
            {
                context.BlisterBlock.Add(new BlisterBlock { ClassifierId = ClassifierId, ClassifierPackingId = ClassifierPackingId });
                context.SaveChanges();
            }
        }

        public ClassifierEditor(DrugClassifierContext context, Guid user)
        {
            _context = context;
            _context.Database.CommandTimeout = 6000;
            _user = user;
            _dictionary = new ClassifierDictionary(context);
        }

        public struct ChangeStatus
        {
            public bool CanRecreate { get; set; }

            public bool NeedMerge { get; set; }

            public string DrugDescription { get; set; }

            public bool OnlyOneProductionInfo { get; set; }

            public bool CanSaveClassifierId { get; set; }
        }
        public class ChangeDescription
        {
            public string Title { get; set; }

            public string OldId { get; set; }

            public string NewId { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }

            public ChangeDescription(string title, string oldValue, string newValue)
            {
                this.OldValue = oldValue;
                this.NewValue = newValue;
                this.Title = title;
            }

            public ChangeDescription(string title, long oldId, string oldValue, long newId, string newValue)
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
            public List<ChangeDescription> ClassifierPacking { get; set; }
            public List<ChangeDescription> RegistrationCertificate { get; set; }
            public List<ChangeDescription> RegistrationCertificateIsBlocked { get; set; }
            public List<ChangeDescription> ChangeUse { get; set; }

        }

        /// <summary>
        /// Проверяем что новое, а что старое
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ClassifierInfoModel CheckClassifier(ClassifierEditorModelJson model)
        {
            model.Clear();

            ClassifierInfoModel infoModel = new ClassifierInfoModel();

            var drug = _dictionary.CheckDrug(model);
            var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark);
            var packer = _dictionary.FindManufacturer(model.Packer);
            Manufacturer ownerRegCert = null;
            if (model.RegistrationCertificate != null)
                ownerRegCert = _dictionary.FindManufacturer(model.RegistrationCertificate.OwnerRegistrationCertificate);

            infoModel.DrugId = drug != null ? drug.Id : 0;
            infoModel.OwnerTradeMarkId = ownerTradeMark != null ? ownerTradeMark.Id : 0;
            infoModel.PackerId = packer != null ? packer.Id : 0;
            infoModel.OwnerRegistrationCertificateId = ownerRegCert != null ? ownerRegCert.Id : 0;

            //if (ownerTradeMark != null)
            //{
            //    var drugClassification = _context.DrugClassification.First(d => d.DrugId == drug.Id && d.OwnerTradeMarkId == ownerTradeMark.Id);
            //    if(drugClassification!=null)
            //    {

            //    }
            //}

            return infoModel;
        }


        public static ClassifierDictionary.DrugProperty GetDrugProperty(ClassifierEditorModelJson model)
        {
            using (var context = new DrugClassifierContext(""))
            {
                return new ClassifierDictionary(context).GetDrugProperty(model);
            }
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
                CanSaveClassifierId = false,
                DrugDescription = string.Empty,
                OnlyOneProductionInfo = false
            };

            var drugProperty = _dictionary.GetDrugProperty(model);

            // Ищем если
            if (drugProperty.IsNew)
                return changeStatus;

            //Ищем Drug
            Drug drug = _dictionary.FindDrug(model);

            if (drug != null)
            {

                changeStatus.NeedMerge = true;
                changeStatus.CanSaveClassifierId = true;

                //Если будет объединение с уже существующей связкой, то сохранить ClassifierId не получится

                var owner = _dictionary.FindManufacturer(model.OwnerTradeMark);
                var packer = _dictionary.FindManufacturer(model.Packer);

                if (owner != null && packer != null)
                {
                    var productionInfo = _dictionary.FindProductionInfo(owner, packer, drug);
                    if (productionInfo != null)
                        changeStatus.CanSaveClassifierId = false;
                }

                StringBuilder builder = new StringBuilder();
                builder.Append(drug.TradeName.Value + " ");
                if (drug.INNGroup != null)
                    builder.Append(drug.INNGroup.Description);
                if (drug.FormProduct != null)
                    builder.Append(drug.FormProduct.Value + " ");
                if (drug.DosageGroup != null)
                    builder.Append(drug.DosageGroup.Description + " ");
                if (drug.Equipment != null)
                    builder.Append(drug.Equipment.Value + " ");
                builder.Append("N" + drug.ConsumerPackingCount);

                changeStatus.DrugDescription = builder.ToString();
            }

            return changeStatus;
        }

        /// <summary>
        /// Получаем список изменений
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChangeInfo GetChanges(ClassifierEditorModelJson model)
        {
            model.Clear();

            List<ChangeDescription> changes = new List<ChangeDescription>();
            List<ChangeDescription> realPackingCount = new List<ChangeDescription>();
            List<ChangeDescription> сlassifierPacking = new List<ChangeDescription>();

            var drug = _context.Drugs.First(d => d.Id == model.DrugId);

            //Изменяемый Drug не найден
            if (drug == null)
                throw new ApplicationException(string.Format("not found Drug Id = {0}", model.DrugId));

            var drugProperty = _dictionary.GetDrugProperty(model);

            //Сравниваем INN
            if ((drug.INNGroup != null && drugProperty.INNGroup == null) ||
                 (drug.INNGroup == null && drugProperty.INNGroup != null) ||
                 (drug.INNGroup != null && drugProperty.INNGroup != null && drug.INNGroup.Description != drugProperty.INNGroup.Description))
            {

                var oldValue = String.Empty;
                if (drug.INNGroup != null)
                    oldValue = drug.INNGroup.Description;

                var newValue = String.Empty;
                if (drugProperty.INNGroup != null)
                    newValue = drugProperty.INNGroup.Description;

                changes.Add(new ChangeDescription("МНН", oldValue, newValue));
            }

            //Сравниваем Комплектацию
            if ((drug.Equipment != null && drugProperty.Equipment == null) ||
                (drug.Equipment == null && drugProperty.Equipment != null) ||
                (drug.Equipment != null && drugProperty.Equipment != null && drug.Equipment.Id != drugProperty.Equipment.Id))
            {

                var oldValue = String.Empty;
                if (drug.Equipment != null)
                    oldValue = drug.Equipment.Value;

                var newValue = String.Empty;
                if (drugProperty.Equipment != null)
                    newValue = drugProperty.Equipment.Value;

                changes.Add(new ChangeDescription("Комплектация", oldValue, newValue));
            }

            if ((drug.DosageGroup != null && drugProperty.DosageGroup == null) ||
                (drug.DosageGroup == null && drugProperty.DosageGroup != null) ||
                (drug.DosageGroup != null && drugProperty.DosageGroup != null && drug.DosageGroup.Id != drugProperty.DosageGroup.Id))
            {
                var oldValue = String.Empty;
                if (drug.DosageGroup != null)
                    oldValue = drug.DosageGroup.Description;

                var newValue = String.Empty;
                if (drugProperty.DosageGroup != null)
                    newValue = drugProperty.DosageGroup.Description;

                if (!String.Equals(oldValue, newValue))
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

            if (drug.UseShortDescription != model.UseShortDescription)
            {
                changes.Add(new ChangeDescription("Короткое описание дозировки", drug.UseShortDescription.ToString(), model.UseShortDescription.ToString()));
            }

            if (drug.DrugType.Id != drugProperty.DrugTypeId)
            {
                var newValue = _context.DrugType.First(t => t.Id == drugProperty.DrugTypeId).Value;
                changes.Add(new ChangeDescription("Тип", drug.DrugType.Value, newValue));
            }

            if (drug.INNGroup != null && drugProperty.INNGroup != null && drug.INNGroup.IsCompound != model.IsCompound)
            {
                changes.Add(new ChangeDescription("Состав", drug.INNGroup.IsCompound ? "Да" : "Нет", model.IsCompound ? "Да" : "Нет"));
            }

            if (drug.INNGroup != null && drugProperty.INNGroup != null && drug.INNGroup.IsCompoundBAA != model.IsCompoundBAA)
            {
                changes.Add(new ChangeDescription("Состав БАД", drug.INNGroup.IsCompoundBAA ? "Да" : "Нет", model.IsCompoundBAA ? "Да" : "Нет"));
            }

            var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ?? new Manufacturer() { Value = model.OwnerTradeMark.Value, Id = 0 };
            var packer = _dictionary.FindManufacturer(model.Packer) ?? new Manufacturer() { Value = model.Packer.Value, Id = 0 };
            var oldOwnerTradeMark = _context.Manufacturer.First(m => m.Id == model.OwnerTradeMarkId);
            var oldPacker = _context.Manufacturer.First(m => m.Id == model.PackerId);

            if (model.RegistrationCertificate != null)
            {
                var ownerRegistrationCertificate = _dictionary.FindManufacturer(model.RegistrationCertificate.OwnerRegistrationCertificate);

                var ownerRegistrationCertificateValue = ownerRegistrationCertificate != null
                    ? ownerRegistrationCertificate.Value
                    : String.Empty;

                Manufacturer oldOwnerRegistrationCertificate = null;

                if (model.RegistrationCertificate.OwnerRegistrationCertificateId != null)
                    oldOwnerRegistrationCertificate = _context.Manufacturer.First(m => m.Id == model.RegistrationCertificate.OwnerRegistrationCertificateId);

                var oldOwnerRegistrationCertificateValue = oldOwnerRegistrationCertificate != null
                    ? oldOwnerRegistrationCertificate.Value
                    : String.Empty;

                if (ownerRegistrationCertificateValue != oldOwnerRegistrationCertificateValue)
                {
                    changes.Add(new ChangeDescription("Владелец Ру", oldOwnerRegistrationCertificateValue, ownerRegistrationCertificateValue));
                }
            }

            if (oldOwnerTradeMark.Value != ownerTradeMark.Value)
            {
                changes.Add(new ChangeDescription("Правообладатель", oldOwnerTradeMark.Value, ownerTradeMark.Value));
            }

            var classificationGeneric = _context.ClassificationGeneric.FirstOrDefault(d =>
                                                                                        d.TradeNameId == drug.TradeNameId &&
                                                                                        d.INNGroupId == drug.INNGroupId &&
                                                                                        d.OwnerTradeMarkId == ownerTradeMark.Id);

            int? model_Generic_id = model.Generic == null || model.Generic.Id == 0 ? null : (int?)model.Generic.Id;
            if (classificationGeneric != null && model_Generic_id != classificationGeneric.GenericId)
            {
                string newValue = null;
                if (model.Generic != null && model.Generic.Id > 0)
                    newValue = _context.Generic.First(t => t.Id == model.Generic.Id).Value;
                changes.Add(new ChangeDescription("Дженерик", classificationGeneric.GenericId == null ? "нет" : classificationGeneric.Generic.Value, newValue == null ? "нет" : newValue));
            }

            //блок для проверки дополнительных характеристик DrugClassification зависимость от правообладателя
            var drugClassification = _context.DrugClassification.FirstOrDefault(d => d.DrugId == drug.Id && d.OwnerTradeMarkId == ownerTradeMark.Id);
            if (drugClassification != null)
            {
                if (model.Nfc.Id != drugClassification.NFCId)
                {
                    var newValue = _context.NFC.First(t => t.Id == model.Nfc.Id).Value;
                    changes.Add(new ChangeDescription("Nfc", drugClassification.NFCId == null ? "нет" : drugClassification.Nfc.Value, newValue == null ? "нет" : newValue));
                }

                long? model_ATCWho_id = model.ATCWho == null ? null : (long?)model.ATCWho.Id;
                if (model_ATCWho_id != drugClassification.ATCWhoId)
                {
                    string newValue = null;
                    if (model.ATCWho != null)
                        newValue = _context.ATCWho.First(t => t.Id == model.ATCWho.Id).Value;
                    changes.Add(new ChangeDescription("ATCWho", drugClassification.ATCWhoId == null ? "нет" : drugClassification.ATCWho.Value, newValue == null ? "нет" : newValue));
                }
                if (model.ATCEphmra.Id != drugClassification.ATCEphmraId)
                {
                    var newValue = _context.ATCEphmra.First(t => t.Id == model.ATCEphmra.Id).Value;
                    changes.Add(new ChangeDescription("ATCEphmra", drugClassification.ATCEphmraId == null ? "нет" : drugClassification.ATCEphmra.Value, newValue == null ? "нет" : newValue));
                }
                long? model_ATCBaa_id = model.ATCBaa == null ? null : (long?)model.ATCBaa.Id;
                if (model_ATCBaa_id != drugClassification.ATCBaaId)
                {
                    string newValue = null;
                    if (model.ATCBaa != null)
                        newValue = _context.ATCBaa.First(t => t.Id == model.ATCBaa.Id).Value;
                    changes.Add(new ChangeDescription("ATCBaa", drugClassification.ATCBaaId == null ? "нет" : drugClassification.ATCBaa.Value, newValue == null ? "нет" : newValue));
                }
                long? model_FTG_id = model.FTG == null ? null : (long?)model.FTG.Id;
                if (model_FTG_id != drugClassification.FTGId)
                {
                    string newValue = null;
                    if (model.FTG != null)
                        newValue = _context.FTG.First(t => t.Id == model.FTG.Id).Value;
                    changes.Add(new ChangeDescription("FTG", drugClassification.FTGId == null ? "нет" : drugClassification.FTG.Value, newValue == null ? "нет" : newValue));
                }

                if (model.IsOtc != drugClassification.IsOtc)
                {
                    changes.Add(new ChangeDescription("IsOtc", drugClassification.IsOtc ? "otc" : "не otc", model.IsOtc ? "otc" : "не otc"));
                }
                if (model.IsRx != drugClassification.IsRx)
                {
                    changes.Add(new ChangeDescription("IsRx", drugClassification.IsRx ? "Rx" : "не Rx", model.IsRx ? "Rx" : "не Rx"));
                }
                if (model.IsExchangeable != drugClassification.IsExchangeable)
                {
                    changes.Add(new ChangeDescription("IsExchangeable", drugClassification.IsExchangeable ? "Взаимозаменяемый" : "не Взаимозаменяемый", model.IsExchangeable ? "Взаимозаменяемый" : "не Взаимозаменяемый"));
                }
                if (model.IsReference != drugClassification.IsReference)
                {
                    changes.Add(new ChangeDescription("IsReference", drugClassification.IsReference ? "Референтный" : "не Референтный", model.IsReference ? "Референтный" : "не Референтный"));
                }
                long? drugClassification_Brand_id = drugClassification.Brand == null ? null : (long?)drugClassification.Brand.Id;
                if (model.Brand.Id != drugClassification_Brand_id)
                {
                    //var newValue = _context.Brand.FirstOrDefault(t => t.Id == model.Brand.Id).Value;
                    changes.Add(new ChangeDescription("Brand", drugClassification_Brand_id == null ? "нет" : drugClassification.Brand.Value, model.Brand.Value));
                }
            }
            else
            {
                changes.Add(new ChangeDescription("Связка Правообладатель новая дополнительные характеристики", "новая", "новая"));
            }

            if (oldPacker.Value != packer.Value)
            {
                changes.Add(new ChangeDescription("Упаковщик", oldPacker.Value, packer.Value));
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
                realPackingCount.Add(new ChangeDescription("", i.ToString(), String.Empty));
            }

            var productionInfo = _context.ProductionInfo.First(p => p.Id == model.ProductionInfoId);

            if (productionInfo.WithoutRegistrationCertificate != model.WithoutRegistrationCertificate)
            {
                var oldUsed = productionInfo.WithoutRegistrationCertificate ? "да" : "нет";
                var newUsed = model.WithoutRegistrationCertificate ? "да" : "нет";

                changes.Add(new ChangeDescription("Без Ру", oldUsed, newUsed));
            }

            if ((productionInfo.ProductionStageId == null && model.ProductionStage != null) ||
                (productionInfo.ProductionStageId != null && model.ProductionStage == null) ||
                (productionInfo.ProductionStageId != null && model.ProductionStage != null && productionInfo.ProductionStageId != model.ProductionStage.Id))
            {
                changes.Add(new ChangeDescription("Стадии производства", productionInfo.ProductionStage?.Value ?? "", model.ProductionStage?.Value ?? ""));
            }

            //Изменения для Локализации
            if (productionInfo.ProductionLocalization?.Id != model.ProductionLocalization?.Id)
            {
                changes.Add(new ChangeDescription("Локализация", productionInfo.ProductionLocalization?.Value, model.ProductionLocalization?.Value));
            }

            var ClassifierInfo = _context.ClassifierInfo.Single(p => p.ProductionInfoId == model.ProductionInfoId);
            var oldClassifierPackings = ClassifierInfo.ClassifierPackings.ToList();
            foreach (var pack in model.ClassifierPackings)//новые
            {
                if (oldClassifierPackings.All(p => !string.Equals(p.Id, pack.Id)))
                {
                    сlassifierPacking.Add(new ChangeDescription("", String.Empty, pack.PackingDescription));
                }
            }

            foreach (var pack in ClassifierInfo.ClassifierPackings)//измененые
            {
                var exClassifierPackings = ClassifierInfo.ClassifierPackings.Where(w => w.Id == pack.Id).FirstOrDefault();
                if (exClassifierPackings != null && exClassifierPackings.PackingDescription != pack.PackingDescription)
                {
                    сlassifierPacking.Add(new ChangeDescription("", exClassifierPackings.PackingDescription, pack.PackingDescription));
                }
            }

            foreach (var pack in oldClassifierPackings)//удалёные
            {
                if (model.ClassifierPackings.All(p => !string.Equals(p.Id, pack.Id)))
                {
                    сlassifierPacking.Add(new ChangeDescription("", pack.PackingDescription, String.Empty));
                }
            }

            #region Собираем информацию про регистрационные сертификаты
            List<ChangeDescription> registrationCertificateChangeDescriptions = new List<ChangeDescription>();
            List<ChangeDescription> registrationCertificateIsBlockedChangeDescriptions = new List<ChangeDescription>();

            //Сначала соберем информацию о новых регистрационных сертификатах
            var regCert = _dictionary.FindCertificate(model.RegistrationCertificate);

            if (regCert == null && model.RegistrationCertificate != null)
                throw new ApplicationException("В редактировании нельзя создавать новый РУ");

            //Если он новый или не присутствует в модели, то значит он новый
            if ((regCert == null && productionInfo.RegistrationCertificate != null) ||
                (regCert != null && productionInfo.RegistrationCertificate == null) ||
                (regCert != null && productionInfo.RegistrationCertificate != null && productionInfo.RegistrationCertificate.Id != regCert.Id))
            {

                var oldNumber = productionInfo.RegistrationCertificate != null
                    ? productionInfo.RegistrationCertificate.Number
                    : String.Empty;

                var newNumber = model.RegistrationCertificate != null
                   ? model.RegistrationCertificate.Number
                   : String.Empty;

                registrationCertificateChangeDescriptions.Add(new ChangeDescription(String.Empty, oldNumber, newNumber));
            }

            if (regCert != null && model.RegistrationCertificate != null &&
                regCert.Id == model.RegistrationCertificate.Id &&
                regCert.IsBlocked != model.RegistrationCertificate.IsBlocked)
            {
                string oldVal = "Серт. " + regCert.Number + (regCert.IsBlocked ? "" : " не") + " заблокирован";
                string newVal = "Серт. " + regCert.Number + (model.RegistrationCertificate.IsBlocked ? "" : " не") + " заблокирован";
                registrationCertificateIsBlockedChangeDescriptions.Add(new ChangeDescription(String.Empty, oldVal, newVal));
            }
            #endregion

            #region Изменения для блокировки Use и Comment
            List<ChangeDescription> changeUse = new List<ChangeDescription>();

            if (productionInfo.Used != model.Used)
            {
                var oldUsed = productionInfo.Used ? "использовать" : "заблокирован";
                var newUsed = model.Used ? "использовать" : "заблокирован";

                changeUse.Add(new ChangeDescription("Блокировка", oldUsed, newUsed));
            }

            if (productionInfo.Comment != model.Comment)
            {
                changeUse.Add(new ChangeDescription("Comment", productionInfo.Comment, model.Comment));
            }
            #endregion

            return new ChangeInfo()
            {
                Items = changes,
                RealPackingCount = realPackingCount,
                ClassifierPacking = сlassifierPacking,
                RegistrationCertificate = registrationCertificateChangeDescriptions,
                RegistrationCertificateIsBlocked = registrationCertificateIsBlockedChangeDescriptions,
                ChangeUse = changeUse
            };

        }

        /// <summary>
        /// Сохранение с новым LKCU c объединением
        /// </summary>
        /// <param name="model"></param>
        /// <param name="saveClassifierId"></param>
        /// <returns></returns>
        public ClassifierInfoModel MergeClassifier(ClassifierEditorModelJson model, bool saveClassifierId)
        {

            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {

                ClassifierInfoModel info = new ClassifierInfoModel();

                //Находим Drug который нам нужно будет объединить
                var fromDrug = _context.Drugs.First(d => d.Id == model.DrugId);
                var fromOwnerTradeMark = _context.Manufacturer.FirstOrDefault(m => m.Id == model.OwnerTradeMarkId);
                var fromPacker = _context.Manufacturer.FirstOrDefault(m => m.Id == model.PackerId);

                //Чистим входную модель.
                model.Clear();

                //Находим по модели Drug, к которому мы будем объединять
                var toDrug = _dictionary.FindDrug(model);

                //Если по модели не удалось найти Drug к которому будем объединять
                if (toDrug == null)
                    throw new ApplicationException("toDrug is null need ReCreate");

                //По модели находим Производителя и Упаковщика так как мы объединяем только по Drug

                var toOwnerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ?? _dictionary.CreateManufacturer(model.OwnerTradeMark);
                var toPacker = _dictionary.FindManufacturer(model.Packer) ?? _dictionary.CreateManufacturer(model.Packer);

                //Теперь пытаемся найти PIс которая будет удаляться
                var productionInfo = _dictionary.FindProductionInfo(fromOwnerTradeMark, fromPacker, fromDrug);

                //Пытаемся найти PIо к которой будем объединять, если такой нет - то создаем её.
                var toProductionInfo = _dictionary.FindProductionInfo(toOwnerTradeMark, toPacker, toDrug);

                if (toProductionInfo != null && productionInfo.Id == toProductionInfo.Id)
                    throw new ApplicationException("Меняем само на себя");

                //Если не будет объединения с другим ProductionInfo то нужно сохранить ProductionInfo и ClassifierId
                var changeProductionInfo = toProductionInfo ?? productionInfo;
                var fromProductionInfo = productionInfo.Copy();

                changeProductionInfo.Drug = toDrug;
                changeProductionInfo.Packer = toPacker;
                changeProductionInfo.OwnerTradeMark = toOwnerTradeMark;
                changeProductionInfo.ProductionStageId = model.ProductionStage?.Id;
                changeProductionInfo.WithoutRegistrationCertificate = model.WithoutRegistrationCertificate;

                //Если Pic - лс и у новой PIC не задан Ru - попытаемся взять его из старого.
                if (toDrug.DrugTypeId == 4)
                {
                    if (changeProductionInfo.RegistrationCertificate == null)
                    {
                        changeProductionInfo.RegistrationCertificateId = fromProductionInfo.RegistrationCertificateId;
                    }
                }

                if (toDrug.DrugTypeId == 1)
                {
                    if (changeProductionInfo.RegistrationCertificate == null && !changeProductionInfo.WithoutRegistrationCertificate)
                    {
                        changeProductionInfo.RegistrationCertificateId = fromProductionInfo.RegistrationCertificateId;
                    }
                }

                _context.SaveChanges();


                if (fromProductionInfo != null)
                {
                    LogAction.Log(_context, fromProductionInfo.Id, LogAction.ActionType.Merge, _user);
                }

                if (toProductionInfo != null)
                {
                    LogAction.Log(_context, toProductionInfo.Id, LogAction.ActionType.Merge, _user);
                }

                //Изменение ProductionInfo и ClassifierId
                ProductionInfoController.ChangeProductionInfo(fromProductionInfo, changeProductionInfo, _user, _context);

                //Если было объединение, то удаляем старый ProductionInfo
                RemoveProductionInfoFromDrug(fromDrug, fromOwnerTradeMark, fromPacker);

                _context.SaveChanges();
                //Добавляем к PIо упаковки из PIс, которых у неё не было. 
                var CI = _context.ClassifierInfo.Where(w => w.ProductionInfoId == changeProductionInfo.Id).Single();
                foreach (var CP in model.ClassifierPackings)
                {
                    CP.Id = 0;
                    var сlassifierPacking = _dictionary.FindClassifierPacking(CI, CP) ??
                                        _dictionary.CreateClassifierPacking(CI, CP);
                }
                _context.SaveChanges();

                transaction.Commit();

                info.DrugId = toDrug.Id;
                info.PackerId = toPacker.Id;
                info.OwnerTradeMarkId = toOwnerTradeMark.Id;
                info.Used = changeProductionInfo.Used;
                info.Comment = changeProductionInfo.Comment;

                return info;

            }
        }

        /// <summary>
        /// Сохранить с новым LKCU без объединения
        /// </summary>
        public ClassifierInfoModel ReCreateClassifier(ClassifierEditorModelJson model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {

                model.Clear();

                var existDrug = _context.Drugs.FirstOrDefault(d => d.Id == model.DrugId);
                var existOwnerTradeMark = _context.Manufacturer.FirstOrDefault(m => m.Id == model.OwnerTradeMarkId);
                var existPacker = _context.Manufacturer.FirstOrDefault(m => m.Id == model.PackerId);

                if (existDrug == null)
                    throw new ApplicationException("Drug not found");

                if (existPacker == null)
                    throw new ApplicationException("Packer not found");

                if (existOwnerTradeMark == null)
                    throw new ApplicationException("OwnerTradeMark not found");

                if (model.ClassifierPackings.Count == 0)
                    throw new ApplicationException("Нет под упаковки");


                //Получаем описание всех параметров Drug
                var drugProperty = _dictionary.GetDrugProperty(model);
                //Ищем Drug
                Drug drug = _dictionary.FindDrug(model);

                if (drug != null)
                    throw new ApplicationException("Данный Drug уже существует нужно воспользоваться объединением");


                drug = _dictionary.CreateDrug(model);
                _context.SaveChanges();

                var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ?? _dictionary.CreateManufacturer(model.OwnerTradeMark);
                var packer = _dictionary.FindManufacturer(model.Packer) ?? _dictionary.CreateManufacturer(model.Packer);


                var changeProductionInfo = _context.ProductionInfo.Single(p => p.Id == model.ProductionInfoId);

                var fromProductionInfo = changeProductionInfo.Copy();

                changeProductionInfo.Drug = drug;
                changeProductionInfo.OwnerTradeMark = ownerTradeMark;
                changeProductionInfo.Packer = packer;
                changeProductionInfo.ProductionStageId = model.ProductionStage?.Id;

                #region Изменения РУ

                RegistrationCertificate certificate = null;

                //Изменяем регистрационные удостоверения
                if ((drug.DrugTypeId != 1 && drug.DrugTypeId != 4) || (changeProductionInfo.OwnerTradeMark.Value == "Unknown" && changeProductionInfo.Packer.Value == "Unknown") || changeProductionInfo.WithoutRegistrationCertificate)
                {
                    //то все РУ удаляются
                    changeProductionInfo.RegistrationCertificateId = null;
                }
                else
                {
                    certificate = _dictionary.FindCertificate(model.RegistrationCertificate);

                    if (certificate == null && drug.DrugTypeId != 4)
                        throw new ApplicationException("Регистрационный сертификат или не задан или не существует в базе");

                    changeProductionInfo.RegistrationCertificate = certificate;
                    if (certificate == null)
                        changeProductionInfo.RegistrationCertificateId = null;

                    if (certificate != null && certificate.IsBlocked != model.RegistrationCertificate.IsBlocked)
                    {
                        certificate.IsBlocked = model.RegistrationCertificate.IsBlocked;
                    }
                }

                #endregion Изменения РУ

                //RealPacking

                AddRealPacking(model.RealPackingList, drug);

                //Удаляем из существующего классификатора
                _context.SaveChanges();

                LogAction.Log(_context, model.ProductionInfoId, LogAction.ActionType.Merge, _user);

                //Изменение ProductionInfo
                ProductionInfoController.ChangeProductionInfo(fromProductionInfo, changeProductionInfo, _user, _context);

                RemoveProductionInfoFromDrug(existDrug, existOwnerTradeMark, existPacker);

                _context.SaveChanges();

                //Собираем ClassifierPackings
                var CI = _context.ClassifierInfo.Where(w => w.ProductionInfoId == changeProductionInfo.Id).Single();
                foreach (var CP in model.ClassifierPackings)
                {
                    _dictionary.CreateClassifierPacking(CI, CP);
                }
                _context.SaveChanges();

                //Если МНН групп новая проверим её на существование
                if (drugProperty.INNGroupNew)
                {
                    CheckInnGroup(drug.INNGroupId.Value);
                }

                //Меняем на новый DrugId
                model.DrugId = drug.Id;

                Update_DrugClassification(model, changeProductionInfo.RegistrationCertificate?.Id ?? 0);
                Update_ClassificationGeneric(drugProperty, model);

                transaction.Commit();

                #region Собираем информация о том, что,изменилось для отчета

                ClassifierInfoModel info = new ClassifierInfoModel
                {
                    KCU = new List<string>(),
                    OwnerTradeMarkIdNew = ownerTradeMark.Id == 0
                };

                info.OwnerTradeMarkId = ownerTradeMark.Id;
                info.PackerIdNew = packer.Id == 0;
                info.PackerId = packer.Id;
                info.OwnerRegistrationCertificateIdNew = false;
                info.OwnerRegistrationCertificateId = 0;
                info.RegistrationCertificateNumber = String.Empty;
                info.RegistrationCertificateNumberNew = false;
                info.RegistrationCertificateIsBlockedOldValue = null;
                info.DrugId = drug.Id;
                info.OwnerTradeMarkId = ownerTradeMark.Id;
                info.PackerId = packer.Id;
                info.Used = model.Used;
                info.Comment = model.Comment;
                info.Drug = drug;
                info.ProductionInfo = changeProductionInfo;

                if (info.DrugId == null && info.Drug.Id > 0)
                {
                    info.DrugId = info.Drug.Id;
                }

                if (certificate != null)
                {

                    info.OwnerRegistrationCertificateIdNew = certificate.OwnerRegistrationCertificate != null &&
                                                              certificate.OwnerRegistrationCertificate.Id == 0;

                    info.OwnerRegistrationCertificateId = certificate.OwnerRegistrationCertificate != null
                        ? certificate.OwnerRegistrationCertificate.Id
                        : 0;

                    info.RegistrationCertificateNumber = certificate.Number;
                    info.RegistrationCertificateNumberNew = certificate != null && certificate.Id == 0;

                    //если у выбранного сертификата сменили режим блокировки, то сохраняем в переменную старое значение. 
                    //иначе null - признак отсутствия изменений.
                    info.RegistrationCertificateIsBlockedOldValue =
                            model.RegistrationCertificate.IsBlocked != certificate.IsBlocked ? (bool?)certificate.IsBlocked : null;
                }


                #endregion Собираем информация о том, что,изменилось для отчета

                //Ищем информацию о сертификате
                return info;

            }
        }

        public void ChangeUse(ClassifierEditorModelJson model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                model.Clear();

                var changeProductionInfo = _context.ProductionInfo.First(p => p.Id == model.ProductionInfoId);
                changeProductionInfo.Used = model.Used;
                changeProductionInfo.Comment = model.Comment;
                LogAction.Log(_context, changeProductionInfo.Id, LogAction.ActionType.Change, _user);

                _context.SaveChanges();

                transaction.Commit();
            }

        }

        //Изменить 
        public ClassifierInfoModel ChangeClassifier(ClassifierEditorModelJson model, bool tryMode)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                model.Clear();

                ClassifierInfoModel info = new ClassifierInfoModel();

                var drug = _context.Drugs.FirstOrDefault(d => d.Id == model.DrugId);

                //Изменяемый Drug не найден
                if (drug == null)
                    throw new ApplicationException(string.Format("not found changes Drug Id = {0}", model.DrugId));

                if (model.ClassifierPackings.Count == 0)
                    throw new ApplicationException("Удалены все упаковки");

                //Получаем описание всех параметров Drug
                var drugProperty = _dictionary.GetDrugProperty(model);

                //Ищем Drug по новым параметрам
                var drugAlreadyExists = _dictionary.FindDrug(model);

                //С тамким характеристиками уже есть другой Drug
                if (drugAlreadyExists != null && drugAlreadyExists.Id != model.DrugId)
                {
                    throw new ApplicationException("Найден другой Drug с такими же характеристиками DrugId:" +
                                                   drugAlreadyExists.Id.ToString());
                }

                //Если что-то поменялось
                drug.ConsumerPackingCount = drugProperty.ConsumerPackingCount;
                drug.UseShortDescription = drugProperty.UseShortDescription;

                drug.FormProduct = drugProperty.FormProduct;
                drug.Equipment = drugProperty.Equipment;
                if (drugProperty.DosageGroup != null)
                {
                    drug.DosageGroup = drugProperty.DosageGroup;
                }
                else
                {
                    drug.DosageGroupId = null;
                }

                if (drugProperty.INNGroup != null)
                {
                    drug.INNGroup = drugProperty.INNGroup;
                    drug.INNGroup.IsCompound = model.IsCompound;
                    drug.INNGroup.IsCompoundBAA = model.IsCompoundBAA;

                }
                else
                {
                    drug.INNGroupId = null;
                }

                drug.TradeName = drugProperty.TradeName;
                drug.DrugTypeId = drugProperty.DrugTypeId;

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
                var ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ??
                                     _dictionary.CreateManufacturer(model.OwnerTradeMark);
                var packer = _dictionary.FindManufacturer(model.Packer) ?? _dictionary.CreateManufacturer(model.Packer);

                var changeProductionInfo = _context.ProductionInfo.First(p => p.Id == model.ProductionInfoId);

                var existproductionInfo = _dictionary.FindProductionInfo(ownerTradeMark, packer, drug);

                if (existproductionInfo != null && changeProductionInfo.Id != existproductionInfo.Id)
                    throw new ApplicationException("У выбранного Drug уже есть такие производитель и упаковщик");
                //Обновляем инфорамцию об упаковках

                //Делаем первоночальную копию перед изменениями
                var productionInfoFrom = changeProductionInfo.Copy();

                changeProductionInfo.OwnerTradeMark = ownerTradeMark;
                changeProductionInfo.Packer = packer;
                changeProductionInfo.Used = model.Used;
                changeProductionInfo.Comment = model.Comment;
                changeProductionInfo.ProductionStageId = model.ProductionStage?.Id;
                changeProductionInfo.ProductionLocalizationId = model.ProductionLocalization?.Id;
                changeProductionInfo.WithoutRegistrationCertificate = model.WithoutRegistrationCertificate;

                #region Изменения РУ

                //Изменяем регистрационные удостоверения

                if ((drug.DrugTypeId != 1 && drug.DrugTypeId != 4) ||
                    (changeProductionInfo.OwnerTradeMark.Value == "Unknown" &&
                     changeProductionInfo.Packer.Value == "Unknown") ||
                    changeProductionInfo.WithoutRegistrationCertificate)
                {
                    //то все РУ удаляются

                    changeProductionInfo.RegistrationCertificateId = null;
                }
                else
                {

                    var regCert = _dictionary.FindCertificate(model.RegistrationCertificate);

                    if (regCert == null && drug.DrugTypeId != 4)
                        throw new ApplicationException(
                            "Регистрационный сертификат или не задан или не существует в базе");

                    changeProductionInfo.RegistrationCertificate = regCert;
                    if (regCert == null)
                        changeProductionInfo.RegistrationCertificateId = null;

                    if (regCert != null && regCert.IsBlocked != model.RegistrationCertificate.IsBlocked)
                    {
                        regCert.IsBlocked = model.RegistrationCertificate.IsBlocked;
                    }
                }

                #endregion Изменения РУ

                //Сохраняем изменения в том числе, чтобы для нового OwnerTradeMarkId или PackerId повяились реальные Id
                LogAction.Log(_context, changeProductionInfo.Id, LogAction.ActionType.Change, _user);

                _context.SaveChanges();

                ProductionInfoController.ChangeProductionInfo(productionInfoFrom, changeProductionInfo, _user, _context);

                _context.SaveChanges();

                var CI = _context.ClassifierInfo.Where(w => w.ProductionInfoId == changeProductionInfo.Id).Single();

                //Удаляем упаковки, которых нету.
                var CP_ids = model.ClassifierPackings.Select(s => s.Id);
                var CP_todel = _context.ClassifierPacking.Where(w => w.ClassifierId == CI.Id).Where(w2 => !CP_ids.Contains(w2.Id)).ToList();
                _context.ClassifierPacking.RemoveRange(CP_todel);
                //ClassifierDictionary CD = new ClassifierDictionary(_context);
                //Собираем ClassifierPacking                
                foreach (var CP in model.ClassifierPackings)
                {
                    _dictionary.CreateClassifierPacking(CI, CP);
                }

                _context.SaveChanges();

                //Обновляем блок [DrugClassification]
                model.PackerId = packer.Id;
                model.OwnerTradeMarkId = ownerTradeMark.Id;
                Update_DrugClassification(model, changeProductionInfo.RegistrationCertificate?.Id ?? 0);
                Update_ClassificationGeneric(drugProperty, model);

                //Обновим таблицу Systematization.DrugClassifierInWork
                _context.DrugClassifierInWork.Where(dcw => (dcw.UserId == _user) && (dcw.DrugId == model.DrugId)).ForEach(dcw => dcw.RealPackingCount = drugProperty.ConsumerPackingCount);

                _context.SaveChanges();

                //Если МНН групп новая проверим её на существование
                if (drugProperty.INNGroupNew)
                {
                    CheckInnGroup(drug.INNGroupId.Value);
                }

                transaction.Commit();

                info.DrugId = drug.Id;
                info.PackerId = packer.Id;
                info.OwnerTradeMarkId = ownerTradeMark.Id;
                info.Used = changeProductionInfo.Used;
                info.Comment = changeProductionInfo.Comment;

                return info;
            }

        }

        private void CheckInnGroup(long innGroupId)
        {
            var existsINNGroup = _context.InnGroupCheckUnique(innGroupId);

            if (existsINNGroup != null && existsINNGroup.Count > 0)
            {
                var result = String.Join(", ", existsINNGroup.Select(e => e.Description).ToArray());

                throw new ApplicationException(String.Format("Такая МНН уже есть {0}", result));
                //Ошибка, такие INNGroup уже есть
            }
        }

        //Редактирвоание Регистрационного сертификата
        public void EditRegistrationCertificate(RegistrationCertificateJson certificate)
        {
            if (certificate.Id == 0)
                throw new ApplicationException("Регистрационный сертификат еще не сохранен");

            var regCert = _context.RegistrationCertificates.Single(r => r.Id == certificate.Id);

            var ownerRegCert = _dictionary.FindManufacturer(certificate.OwnerRegistrationCertificate) ??
                               _dictionary.CreateManufacturer(certificate.OwnerRegistrationCertificate);

            if (string.IsNullOrEmpty(certificate.Number))
                throw new ApplicationException("Должен быть указан номер РУ");

            var circulationPeriod = _dictionary.FindOrCreateCirculationPeriod(certificate.CirculationPeriod);

            regCert.Number = certificate.Number;
            regCert.CirculationPeriod = circulationPeriod;
            regCert.RegistrationDate = certificate.RegistrationDate.HasValue ? certificate.RegistrationDate.Value.Date : (DateTime?)null;
            regCert.ExpDate = certificate.ExpDate.HasValue ? certificate.ExpDate.Value.Date : (DateTime?)null;
            regCert.ReissueDate = certificate.ReissueDate.HasValue ? certificate.ReissueDate.Value.Date : (DateTime?)null;
            regCert.OwnerRegistrationCertificate = ownerRegCert;
            regCert.IsBlocked = certificate.IsBlocked;

            _context.SaveChanges();
        }

        public ClassifierInfoModel AddClassifier(ClassifierEditorModelJson model, bool tryMode)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = Add(model, tryMode);

                    transaction.Commit();

                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Удалить ЛП
        /// </summary>
        /// <param name="model"></param>
        public void DeleteDrug(ClassifierInfoModel model)
        {
            if (!model.IsDrugNew)
                return;

            var drug = _context.Drugs.Find(model.DrugId);
            if (drug != null)
            {
                _context.Drugs.Remove(drug);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Добавляем новый классификатор
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tryMode">Предварительная подготовка, если true. Запись в БД, если false.</param>
        /// <returns></returns>
        private ClassifierInfoModel Add(ClassifierEditorModelJson model, bool tryMode)
        {
            model.Clear();
            var drugProperty = _dictionary.GetDrugProperty(model);

            #region Ищем Drug
            Drug drug = null;

            // Ищем если есть
            if (!drugProperty.IsNew)
                drug = _dictionary.FindDrug(model);

            if (drug == null) // проставляем новый Drug, если окончательно решили добавить ЛП
            {
                drug = _dictionary.CreateDrug(model);

                _context.SaveChanges();

                model.DrugId = drug.Id;
                drugProperty.IsNew = true;
            }
            #endregion

            #region OnwerTradeMark, Packer
            Manufacturer ownerTradeMark = _dictionary.FindManufacturer(model.OwnerTradeMark) ?? _dictionary.CreateManufacturer(model.OwnerTradeMark);
            Manufacturer packer = _dictionary.FindManufacturer(model.Packer) ?? _dictionary.CreateManufacturer(model.Packer);

            //Я не знаю почему, но в моделе остаётся код от того который был введён при выборе из листа, поэтому меняем код модели на код драга 2019,01,31
            model.DrugId = drug.Id;

            model.OwnerTradeMarkId = ownerTradeMark.Id;
            model.PackerId = packer.Id;
            #endregion

            #region Ищем сертификаты
            RegistrationCertificate certificate = null;

            if (!model.WithoutRegistrationCertificate && model.RegistrationCertificate == null && (ownerTradeMark.Id.ToString() != "6" || packer.Id.ToString() != "6") && drug.DrugTypeId == 1)
                throw new ApplicationException("не задан регистрационный сертификат");

            if (model.RegistrationCertificate != null)
            {
                certificate = _dictionary.FindCertificate(model.RegistrationCertificate) ?? _dictionary.CreateCertificate(model.RegistrationCertificate);

                //Только для новых сертификатов 
                if (certificate != null && certificate.Id == 0)
                {
                    Manufacturer ownerRegistrationCertificate = _dictionary.FindManufacturer(model.RegistrationCertificate.OwnerRegistrationCertificate) ?? _dictionary.CreateManufacturer(model.RegistrationCertificate.OwnerRegistrationCertificate);
                    certificate.OwnerRegistrationCertificate = ownerRegistrationCertificate;
                }
            }
            #endregion

            #region Ищем ProductionInfo
            var productionInfo = _dictionary.FindProductionInfo(ownerTradeMark, packer, drug);

            if (productionInfo == null)
            {
                productionInfo = new ProductionInfo()
                {
                    Drug = drug,
                    OwnerTradeMark = ownerTradeMark,
                    Packer = packer,
                    Used = model.Used,
                    Comment = model.Comment,
                    kofPriceGZotkl = 5,
                    WithoutRegistrationCertificate = model.WithoutRegistrationCertificate,
                    RegistrationCertificate = certificate,
                    ProductionStageId = model.ProductionStage?.Id,
                    ProductionLocalizationId = model.ProductionLocalization?.Id
                };

                _context.ProductionInfo.Add(productionInfo);
            }
            #endregion

            #region собираем RealPacking
            AddRealPacking(model.RealPackingList, drug);
            #endregion

            #region Сохранение
            if (!tryMode)
            {
                _context.SaveChanges();

                ProductionInfoController.ChangeProductionInfo(null, productionInfo, _user, _context);

                _context.SaveChanges();

                #region Собираем ClassifierPacking

                var CI = _context.ClassifierInfo.Where(w => w.ProductionInfoId == productionInfo.Id).Single();

                bool IsOtherClassifier = CI.Id != model.ClassifierId; // если при добавлении используется другой классифер, отличный от редактируемого

                foreach (var CP in model.ClassifierPackings)
                {
                    if (IsOtherClassifier)
                        CP.Id = 0;

                    _dictionary.CreateClassifierPacking(CI, CP);
                }
                _context.SaveChanges();

                #endregion

                var dc = _context.DrugClassification.Single(c => c.DrugId == productionInfo.DrugId && c.OwnerTradeMarkId == productionInfo.OwnerTradeMarkId);

                if (dc.NFCId == null)
                {
                    dc.NFCId = model.Nfc.Id;
                }

                // добавляем блистеровку
                AddBlisterBlock(CI, _context);

                LogAction.Log(_context, productionInfo.Id, LogAction.ActionType.Add, _user);

                _context.SaveChanges();

                model.OwnerTradeMarkId = ownerTradeMark.Id;
                model.PackerId = packer.Id;

                Update_DrugClassification(model, productionInfo.RegistrationCertificate?.Id ?? 0);
                Update_ClassificationGeneric(drugProperty, model);
                //Если МНН групп новая проверим её на существование
                if (drugProperty.INNGroupNew)
                {
                    CheckInnGroup(drug.INNGroupId.Value);
                }
            }
            #endregion

            #region Собираем информацию о том, что изменилось для отчета
            ClassifierInfoModel info = new ClassifierInfoModel
            {
                KCU = new List<string>(),
                OwnerTradeMarkIdNew = ownerTradeMark.Id == 0
            };

            info.OwnerTradeMarkId = ownerTradeMark.Id;
            info.PackerIdNew = packer.Id == 0;
            info.PackerId = packer.Id;
            info.OwnerRegistrationCertificateIdNew = false;
            info.OwnerRegistrationCertificateId = 0;
            info.RegistrationCertificateNumber = String.Empty;
            info.RegistrationCertificateNumberNew = false;
            info.RegistrationCertificateIsBlockedOldValue = null;
            info.DrugId = drug.Id;
            info.OwnerTradeMarkId = ownerTradeMark.Id;
            info.PackerId = packer.Id;
            info.Used = model.Used;
            info.Comment = model.Comment;
            info.Drug = drug;
            info.IsDrugNew = drugProperty.IsNew;

            info.ProductionInfo = productionInfo;

            if (info.DrugId == null && info.Drug.Id > 0)
            {
                info.DrugId = info.Drug.Id;
            }

            if (certificate != null)
            {

                info.OwnerRegistrationCertificateIdNew = certificate.OwnerRegistrationCertificate != null && certificate.OwnerRegistrationCertificate.Id == 0;

                info.OwnerRegistrationCertificateId = certificate.OwnerRegistrationCertificate != null
                    ? certificate.OwnerRegistrationCertificate.Id
                    : 0;

                info.RegistrationCertificateNumber = certificate.Number;
                info.RegistrationCertificateNumberNew = certificate != null && certificate.Id == 0;

                //если у выбранного сертификата сменили режим блокировки, то сохраняем в переменную старое значение. 
                //иначе null - признак отсутствия изменений.
                info.RegistrationCertificateIsBlockedOldValue = model.RegistrationCertificate.IsBlocked != certificate.IsBlocked ? (bool?)certificate.IsBlocked : null;
            }
            #endregion

            return info;
        }

        private void Update_ClassificationGeneric(ClassifierDictionary.DrugProperty drugProperty, ClassifierEditorModelJson model)
        {
            if (drugProperty.INNGroup == null)
                return;

            var classificationGeneric = _context.ClassificationGeneric.SingleOrDefault(g =>
                g.TradeNameId == drugProperty.TradeName.Id &&
                g.INNGroupId == drugProperty.INNGroup.Id &&
                g.OwnerTradeMarkId == model.OwnerTradeMarkId);

            if (classificationGeneric != null && model.Generic != null && model.Generic.Id > 0)
            {
                classificationGeneric.GenericId = model.Generic.Id;
                classificationGeneric.DateEdit = DateTime.Now;
                classificationGeneric.UserId = _user.ToString();
            }

            _context.SaveChanges();
        }

        private void AddRealPacking(List<RealPackingCountJson> realPackingList, Drug drug)
        {
            if (realPackingList != null)
            {
                var exists = _context.RealPacking.Where(w => w.DrugId == drug.Id).Select(s => s.RealPackingCount);
                foreach (var pack in realPackingList)
                {
                    if (!exists.Contains(pack.RealPackingCount))
                    {
                        _context.RealPacking.Add(new RealPacking()
                        {
                            Drug = drug,
                            RealPackingCount = pack.RealPackingCount
                        });
                    }
                }
            }
        }

        private void Update_DrugClassification(ClassifierEditorModelJson model, long RegistrationCertificateId)
        {
            var drugClassificationNew = _context.DrugClassification.SingleOrDefault(c => c.DrugId == model.DrugId && c.OwnerTradeMarkId == model.OwnerTradeMarkId);
            bool isnew = false;
            long? ATCBaaId = null;
            long? ATCWhoId = null;
            long? FTGId = null;
            if (model.ATCBaa != null && model.ATCBaa.Id > 0)
                ATCBaaId = model.ATCBaa.Id;
            if (model.ATCWho != null && model.ATCWho.Id > 0)
                ATCWhoId = model.ATCWho.Id;
            if (model.FTG != null && model.FTG.Id > 0)
                FTGId = model.FTG.Id;
            if (model.Brand.Id > 0)
            {
                // Выбираем Бренд по Id
                var IssetBrand = _context.Brand.SingleOrDefault(b => b.Id == model.Brand.Id);
                //Если в Бренде проставлено значение используется в Goods но не проставлено использовать в ЛС то проставляем это поле
                if (IssetBrand != null && model.DrugId>0) {
                    if(IssetBrand.UseClassifier !=true && IssetBrand.UseGoodsClassifier == true)
                        IssetBrand.UseClassifier = true;
                        _context.SaveChanges();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Brand.Value))
                {
                    var new_brand = _context.Brand.SingleOrDefault(b => b.Value == model.Brand.Value && b.UseClassifier);
                    if (new_brand == null)
                    {
                        //Поищем среди Goods
                        new_brand = _context.Brand.SingleOrDefault(b => b.Value == model.Brand.Value && b.UseGoodsClassifier);

                        if (new_brand != null)
                        {
                            //Если нашли, то отметим как еще и для ЛС
                            new_brand.UseClassifier = true;
                        }
                        else
                        {
                            new_brand = _context.Brand.Add(new Brand() { Value = model.Brand.Value, UseClassifier = true });
                            
                        }
                        _context.SaveChanges();
                    }
                    model.Brand.Id = new_brand.Id;
                }
            }

            //Если для новой связки еще не было характеристик, то их нужно добавить
            if (drugClassificationNew == null)
            {
                drugClassificationNew = new DrugClassification();
                drugClassificationNew.DrugId = model.DrugId;
                drugClassificationNew.OwnerTradeMarkId = model.OwnerTradeMarkId;
                drugClassificationNew.FilledParametersDate = DateTime.Now;
                isnew = true;
            }
            drugClassificationNew.NFCId = model.Nfc.Id;

            //Изменение всех по РУ
            if (RegistrationCertificateId > 0)
            {
                var certs = _context.ProductionInfo.Where(w => w.RegistrationCertificateId == RegistrationCertificateId);
                var DC_certs = _context.DrugClassification
                    .Where(w => w.ATCWhoId == drugClassificationNew.ATCWhoId && w.FTGId == drugClassificationNew.FTGId && w.ATCEphmraId == drugClassificationNew.ATCEphmraId)
                    .Where(w2 => certs.Count(wc => wc.DrugId == w2.DrugId && wc.OwnerTradeMarkId == w2.OwnerTradeMarkId) > 0);
                foreach (var dc_upd in DC_certs)
                {
                    dc_upd.ATCBaaId = ATCBaaId;
                    dc_upd.ATCEphmraId = model.ATCEphmra.Id;
                    dc_upd.ATCWhoId = ATCWhoId;
                    dc_upd.FTGId = FTGId;
                    dc_upd.IsOtc = model.IsOtc;
                    dc_upd.IsRx = model.IsRx;
                }
            }

            drugClassificationNew.ATCBaaId = ATCBaaId;
            drugClassificationNew.ATCEphmraId = model.ATCEphmra.Id;
            drugClassificationNew.ATCWhoId = ATCWhoId;
            drugClassificationNew.FTGId = FTGId;
            drugClassificationNew.IsOtc = model.IsOtc;
            drugClassificationNew.IsRx = model.IsRx;
            drugClassificationNew.IsReference = model.IsReference;
            drugClassificationNew.IsExchangeable = model.IsExchangeable;
            drugClassificationNew.LastChangedParametersDate = DateTime.Now;
            drugClassificationNew.LastChangedParametersUserId = null;
            drugClassificationNew.BrandId = model.Brand.Id;

            //Меняем Brand у всех комбинаций TradeName + OwnerTradeMark

            var drugs = _context.Drugs.Where(d => d.TradeNameId == model.TradeName.Id);

            foreach (var drugClassification in _context.DrugClassification.Where(dc => drugs.Any(d => d.Id == dc.DrugId) && dc.OwnerTradeMarkId == model.OwnerTradeMarkId))
            {
                drugClassification.BrandId = model.Brand.Id;
            }

            if (isnew)
            {
                _context.DrugClassification.Add(drugClassificationNew);
            }

            _context.SaveChanges();
        }

        #region DeleteOperation


        private void RemoveDrug(Drug drug)
        {
            if (drug.ProductionInfo.Count == 0)
            {
                var realPackingList = drug.RealPacking.ToList();
                _context.RealPacking.RemoveRange(realPackingList);
                _context.Drugs.Remove(drug);
            }
        }


        /// <summary>
        /// Удаляем ProductionInfo из старого Drug после сохранения с новым LKCU 
        /// </summary>
        /// <param name="drug"></param>
        /// <param name="ownerTradeMark"></param>
        /// <param name="packer"></param>
        private void RemoveProductionInfoFromDrug(Drug drug, Manufacturer ownerTradeMark, Manufacturer packer)
        {
            var existsproductionInfo = _dictionary.FindProductionInfo(ownerTradeMark, packer, drug);

            if (existsproductionInfo != null)
            {
                existsproductionInfo.RegistrationCertificate = null;
                existsproductionInfo.Drug = null;
                _context.ProductionInfo.Remove(existsproductionInfo);
            }

            RemoveDrug(drug);


        }
        #endregion DeleteOperation


    }

}
