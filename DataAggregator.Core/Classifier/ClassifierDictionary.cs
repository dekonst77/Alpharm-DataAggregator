using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAggregator.Core.Classifier
{
    public class ClassifierDictionary
    {
        private readonly DrugClassifierContext _context;

        public ClassifierDictionary(DrugClassifierContext context)
        {
            _context = context;
        }

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
                    innGroup = FindInnGroup(innList, model);
                }

                if (innGroup == null)
                {
                    innGroup = CreateInnGroup(innList, model.InnGroupDosageDescription, model.IsCompound, model.IsCompoundBAA);

                }
            }
            return innGroup;
        }

        public ClassifierPacking CreateClassifierPacking(ClassifierInfo CI, ClassifierPacking CP)
        {

            Packing consumerPacking = FindOrCreatePacking(CP.ConsumerPacking);
            Packing primaryPacking = FindOrCreatePacking(CP.PrimaryPacking);

            ClassifierPacking CP_Upd = new ClassifierPacking();
            if (CP.Id > 0)
            {
                CP_Upd = _context.ClassifierPacking.Where(w => w.Id == CP.Id).Single();
            }
            //CP_Upd.ConsumerPacking = consumerPacking;
            //CP_Upd.PrimaryPacking = primaryPacking;
            CP_Upd.CountInPrimaryPacking = CP.CountInPrimaryPacking;
            CP_Upd.CountPrimaryPacking = CP.CountPrimaryPacking;
            CP_Upd.PackingDescription = CP.PackingDescription;
            CP_Upd.ClassifierId = CI.Id;

            CP_Upd.PrimaryPackingId = primaryPacking.Id;
            CP_Upd.ConsumerPackingId = consumerPacking.Id;

            if (CP_Upd.Id == 0)
            {
                _context.ClassifierPacking.Add(CP_Upd);
            }

            return CP;
        }

        public ClassifierPacking FindClassifierPacking(ClassifierInfo CI, ClassifierPacking CP)
        {

            Packing consumerPacking = FindOrCreatePacking(CP.ConsumerPacking);
            Packing primaryPacking = FindOrCreatePacking(CP.PrimaryPacking);

            if (consumerPacking != null && consumerPacking.Id == 0)
                return null;

            if (primaryPacking != null && primaryPacking.Id == 0)
                return null;

            var ClassifierPackings = _context.ClassifierPacking.Where(d => d.ClassifierId == CI.Id);

            if (consumerPacking != null && consumerPacking.Id > 0)
            {
                ClassifierPackings = ClassifierPackings.Where(d => d.ConsumerPackingId == consumerPacking.Id);
            }

            if (consumerPacking == null)
                ClassifierPackings = ClassifierPackings.Where(d => !d.ConsumerPackingId.HasValue);


            if (primaryPacking != null && primaryPacking.Id > 0)
            {
                ClassifierPackings = ClassifierPackings.Where(d => d.PrimaryPackingId == primaryPacking.Id);
            }

            if (primaryPacking == null)
                ClassifierPackings = ClassifierPackings.Where(d => !d.PrimaryPackingId.HasValue);


            if (string.IsNullOrEmpty(CP.PackingDescription))
            {
                ClassifierPackings = ClassifierPackings.Where(d => d.PackingDescription == null);
            }
            else
            {
                ClassifierPackings = ClassifierPackings.Where(d => string.Equals(d.PackingDescription, CP.PackingDescription));
            }


            var drugClassifierPackingFound = ClassifierPackings.Where(d => d.CountInPrimaryPacking == CP.CountInPrimaryPacking && d.CountPrimaryPacking == CP.CountPrimaryPacking).ToList();


            if (drugClassifierPackingFound.Count > 1)
            {
                return drugClassifierPackingFound.First();
                //Временно отключено
            }

            return null;


        }



        private Packing FindPacking(Packing packing)
        {
            if (packing == null)
                return new Packing() { Id = null, Value = null };

            if (packing.Id > 0)
                return _context.Packings.First(p => p.Id == packing.Id);

            if (!string.IsNullOrEmpty(packing.Value))
            {
                var foundPackings = _context.Packings.Where(p => p.Value == packing.Value).FirstOrDefault();

                return foundPackings;
            }

            return null;
        }

        private Packing FindOrCreatePacking(Packing packing)
        {
            if (packing == null || (packing.Id == null && packing.Value == null))
                return new Packing() { Id = null, Value = null };

            Packing findpacking = FindPacking(packing);
            if (findpacking != null && findpacking.Id > 0)
                packing = findpacking;


            if (packing.Id == null && !string.IsNullOrEmpty(packing.Value))
            {
                packing = new Packing()
                {
                    Value = packing.Value
                };
                var ADD_context = new DrugClassifierContext(_context);

                ADD_context.Packings.Add(packing);
                ADD_context.SaveChanges();
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

        private INNGroup CreateInnGroup(List<INN> innList, string description, bool isCompound, bool isCompoundBAA)
        {
            var innGroup = new INNGroup { Description = description, INNGroup_INN = new List<INNGroup_INN>() };

            for (var i = 0; i < innList.Count; i++)
            {
                innGroup.INNGroup_INN.Add(new INNGroup_INN() { INN = innList[i], Order = i + 1 });
            }

            innGroup.IsCompound = isCompound;
            innGroup.IsCompoundBAA = isCompoundBAA;

            _context.INNGroups.Add(innGroup);

            return innGroup;
        }

        /// <summary>
        /// Список INN c учетом порядка
        /// </summary>
        /// <param name="innList"></param>
        /// <returns></returns>
        private INNGroup FindInnGroup(List<INN> innList, ClassifierEditorModelJson model)
        {

            var findInnGroups = new List<long>();


            for (var i = 0; i < innList.Count; i++)
            {
                var innId = innList[i].Id;
                var order = i + 1;
                var inns = _context.INNGroup_INN.Where(g => g.Order == order && g.INNId == innId);

                if (i == 0)
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

                if (model.DrugId > 0)
                {
                    var drug = _context.Drugs.Single(d => d.Id == model.DrugId);

                    if (findInnGroups.Any(i => i == drug.INNGroupId))
                    {
                        return _context.INNGroups.First(g => g.Id == drug.INNGroupId);
                    }

                }

                return _context.INNGroups.First(g => g.Id == innGroupId);
            }

            if (findInnGroups.Count() > 1)
            {
                //Временно отключил
                //    throw new ApplicationException("ups many innGroups found");

                //Если нашлись дубли, то убираем дубли путем поиска исходного написания
                if (model.DrugId < 0)
                {
                    var drug = _context.Drugs.Single(d => d.Id == model.DrugId);

                    if (drug.INNGroupId.HasValue && findInnGroups.Contains(drug.INNGroupId.Value))
                    {
                        return _context.INNGroups.First(g => g.Id == drug.INNGroupId.Value);
                    }
                }

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

        private Equipment FindEquipment(ClassifierEditorModelJson model)
        {
            if (model.Equipment == null)
                return null;

            Equipment equipment = null;

            if (model.Equipment.Id > 0)
            {
                equipment = _context.Equipment.First(e => e.Id == model.Equipment.Id);
            }
            else if (!string.IsNullOrEmpty(model.Equipment.Value))
            {
                var equipments = _context.Equipment.Where(e => e.Value == model.Equipment.Value).ToList();

                if (!equipments.Any())
                    equipments = _context.Equipment.Local.Where(e => e.Value == model.Equipment.Value).ToList();

                if (equipments.Count > 1)
                    throw new ApplicationException("нарушение целостности комплектации");

                if (equipments.Count == 1)
                    return equipments.First();
            }

            return equipment;

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

                if (!tradeNames.Any())
                    tradeNames = _context.TradeNames.Local.Where(tn => tn.Value == model.TradeName.Value).ToList();

                if (tradeNames.Count > 1)
                    throw new ApplicationException("нарушение целостности торгового наименования");

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

        private Equipment FindOrCreateEquipment(ClassifierEditorModelJson model)
        {
            Equipment equipment = FindEquipment(model);


            if (model.Equipment != null && equipment == null && !string.IsNullOrEmpty(model.Equipment.Value))
            {
                equipment = new Equipment() { Value = model.Equipment.Value };
                _context.Equipment.Add(equipment);
            }

            return equipment;

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

            //Тут мы берем все дозировки и начинаем накладывать фильтры
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

            List<DosageGroup> dosageInnGroups = null;

            for (int i = 0; i < dosageList.Count; i++)
            {
                var dosage = dosageList[i];

                if (dosage.Dosage == null && string.IsNullOrEmpty(dosage.DosageCount))
                    continue;

                var dosageId = dosage.Dosage != null ? dosage.Dosage.Id : (long?)null;

#if !DEBUG
                List<INNDosage> inndosagesList = _context.INNDosage.Where(inndosage => inndosage.Order == dosage.Order &&
                                                                       inndosage.DosageId == dosageId &&
                                                                       inndosage.DosageCount == dosage.DosageCount).ToList();

                dosageInnGroups = inndosagesList.Select(idl => idl.DosageGroup).ToList();
#else
                dosageInnGroups = _context.DosageGroups
                    .Join(_context.INNDosage, lefttbl => lefttbl.Id, righttbl => righttbl.DosageGroupId, (lefttbl, righttbl) => new { lefttbl, righttbl })
                    .Where(t => t.righttbl.Order == dosage.Order && t.righttbl.DosageId == dosageId && t.righttbl.DosageCount == dosage.DosageCount)
                    .Select(t => t.lefttbl)
                    .ToList();
#endif
                dosageGroupsFound = dosageGroupsFound.Intersect(dosageInnGroups).ToList();

                if (dosageGroupsFound.Count == 0)
                    return null;
            }

            // кол-во дозировок
            var dosageInnCount = dosageList.Count(c => c.Dosage != null);

            // список групп дозировок            
            var dosageGroupsList = string.Join(",", dosageGroupsFound.Select(t => t.Id).ToList());

            dosageGroupsFound = GetDosageGroups(dosageGroupsList, dosageInnCount);
            //dosageGroupsFound = dosageGroupsFound.Where(d => d.INNDosages != null && d.INNDosages.Count == dosageInnCount).ToList();

            if (dosageGroupsFound.Count == 1)
                return dosageGroupsFound.First();

            if (dosageGroupsFound.Count > 1)
            {
                //Попробуем взять ту, которая изначально была в модели, если такого не будет то берём первую

                if (model.DrugId > 0)
                {
                    var oldDosageGroupId = _context.Drugs.First(d => d.Id == model.DrugId).DosageGroupId;

                    var foundDosage = dosageGroupsFound.FirstOrDefault(d => d.Id == oldDosageGroupId);

                    if (foundDosage != null)
                        return foundDosage;
                }

                return dosageGroupsFound.First();
                //Временно отключено
                //throw new ApplicationException("нарушение целостности группировки дозировок");
            }

            return null;
        }

        /// <summary>
        /// Получить список дозировок
        /// </summary>
        /// <param name="dosageGroupsList">список ID групп дозировок</param>
        /// <param name="dosageInnCount">кол-во одинаковых дозировок в группе</param>
        /// <returns></returns>
        private List<DosageGroup> GetDosageGroups(string dosageGroupsList, int dosageInnCount)
        {
            List<DosageGroup> dosageGroupsFound;

            try
            {
                _context.Database.Connection.Open();

                // Create a SQL command to execute the sproc
                using (var cmd = _context.Database.Connection.CreateCommand())
                {
                    cmd.CommandText = "[Classifier].[GetDosageGroups]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("dosageGroupsList", dosageGroupsList));
                    cmd.Parameters.Add(new SqlParameter("dosageInnCount", dosageInnCount));

                    // Run the sproc
                    var reader = cmd.ExecuteReader();

                    // Read DosageGroups from the first result set
                    dosageGroupsFound = ((IObjectContextAdapter)_context)
                       .ObjectContext
                       .Translate<DosageGroup>(reader, "DosageGroups", MergeOption.AppendOnly)
                       .ToList();
                }
            }
            finally
            {
                _context.Database.Connection.Close();
            }

            return dosageGroupsFound;
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
                ShortDescription = model.ShortDosageGroupDescription,
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

        //Поиск Дозировки
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

        public Manufacturer FindManufacturer(DictionaryJson manufacturerJson)
        {

            if (manufacturerJson.Id > 0)
                return _context.Manufacturer.First(p => p.Id == manufacturerJson.Id);

            if (!string.IsNullOrEmpty(manufacturerJson.Value))
            {
                var foundManufacturers = _context.Manufacturer.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count == 0)
                    foundManufacturers = _context.Manufacturer.Local.Where(p => p.Value == manufacturerJson.Value).ToList();

                if (foundManufacturers.Count > 1)
                    throw new ApplicationException("Нарушение целостности справочника производителей");

                if (foundManufacturers.Count == 1)
                    return foundManufacturers.First();

            }

            return null;
        }

        public Manufacturer CreateManufacturer(DictionaryJson manufacturerJson)
        {
            var manufacturer = FindManufacturer(manufacturerJson);

            if (manufacturer == null && !string.IsNullOrEmpty(manufacturerJson.Value))
            {

                /// var keyOrder = _context.Manufacturer.Max(k => k.KeyOrder);
                /// var localKeyOrder = _context.Manufacturer.Local.ToList().Max(k => k.KeyOrder);

                /// long maxOrder = 1;

                ///if (keyOrder.HasValue)
                ///    maxOrder = keyOrder.Value;

                ///if (localKeyOrder.HasValue)
                ///    maxOrder = localKeyOrder.Value;

                ///if (keyOrder.HasValue && localKeyOrder.HasValue)
                ///  maxOrder = keyOrder.Value > localKeyOrder.Value ? keyOrder.Value : localKeyOrder.Value;

                manufacturer = new Manufacturer()
                {
                    Value = manufacturerJson.Value,
                    /// Key = (maxOrder + 1).ToString(),
                    /// KeyOrder = maxOrder + 1
                };

                _context.Manufacturer.Add(manufacturer);
            }

            return manufacturer;
        }





        public string GetDesription(ClassifierPacking packing)
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


        #region RegistrationCertificate

        public RegistrationCertificate FindCertificate(RegistrationCertificateJson registrationCeritifacte)
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
            {
                regCertFound = regCertFound.Where(rc => !rc.ReissueDate.HasValue);
            }

            if (registrationCeritifacte.OwnerRegistrationCertificate != null && registrationCeritifacte.OwnerRegistrationCertificate.Id >= 0)
            {
                registrationCeritifacte.OwnerRegistrationCertificateId = registrationCeritifacte.OwnerRegistrationCertificate.Id;
            }
            if (registrationCeritifacte.OwnerRegistrationCertificateId.HasValue)
            {
                regCertFound = regCertFound.Where(rc => rc.OwnerRegistrationCertificateId == registrationCeritifacte.OwnerRegistrationCertificateId.Value);
            }
            else
            {
                regCertFound = regCertFound.Where(rc => rc.OwnerRegistrationCertificateId == null);
            }

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
                    throw new ApplicationException("нарушение целостности периода перевыпуска");

                if (periods.Count == 1)
                    return periods.First();
            }

            return period;
        }

        public CirculationPeriod FindOrCreateCirculationPeriod(DictionaryJson periodJson)
        {
            CirculationPeriod period = FindCirculationPeriod(periodJson);

            if (period == null && periodJson != null && !string.IsNullOrEmpty(periodJson.Value))
            {
                period = new CirculationPeriod { Value = periodJson.Value };
                _context.CirculationPeriod.Add(period);
            }

            return period;
        }

        public RegistrationCertificate CreateCertificate(RegistrationCertificateJson registrationCeritifacte)
        {
            if (string.IsNullOrEmpty(registrationCeritifacte.Number))
                return null;

            // проверка на сушествующий номер РУ
            var regCert = _context.RegistrationCertificates.Where(t => t.Number == registrationCeritifacte.Number).FirstOrDefault();

            if (regCert != null)
                return regCert;

            var circulationPeriod = FindOrCreateCirculationPeriod(registrationCeritifacte.CirculationPeriod);

            regCert = new RegistrationCertificate()
            {
                Number = registrationCeritifacte.Number,
                CirculationPeriod = circulationPeriod,
                RegistrationDate = registrationCeritifacte.RegistrationDate.HasValue ? registrationCeritifacte.RegistrationDate.Value.Date : (DateTime?)null,
                ExpDate = registrationCeritifacte.ExpDate.HasValue ? registrationCeritifacte.ExpDate.Value.Date : (DateTime?)null,
                ReissueDate = registrationCeritifacte.ReissueDate.HasValue ? registrationCeritifacte.ReissueDate.Value.Date : (DateTime?)null,
                IsBlocked = registrationCeritifacte.IsBlocked

            };

            _context.RegistrationCertificates.Add(regCert);

            return regCert;
        }

        #endregion RegistrationCertificate



        #region ProductionInfo

        public ProductionInfo FindProductionInfo(Manufacturer ownerTradeMark, Manufacturer packer, Drug drug)
        {
            ProductionInfo productionInfo = null;

            if (drug.Id > 0 && ownerTradeMark?.Id > 0 && packer?.Id > 0)
            {
                var productionInfoFound = _context.ProductionInfo.Where(p => p.OwnerTradeMarkId == ownerTradeMark.Id && p.PackerId == packer.Id && p.DrugId == drug.Id).ToList();

                if (productionInfoFound.Count > 1)
                    throw new ApplicationException("Нарушение целостности ProductionInfo");

                if (productionInfoFound.Count == 1)
                    productionInfo = productionInfoFound.First();
            }


            return productionInfo;
        }

        public ProductionInfo CreateProductionInfo(Drug drug, Manufacturer ownerTradeMark, Manufacturer packer, bool used, string Comment)
        {
            var productionInfo = new ProductionInfo()
            {
                Drug = drug,
                OwnerTradeMark = ownerTradeMark,
                Packer = packer,
                Used = used,
                Comment = Comment,
                kofPriceGZotkl = 5
            };
            _context.ProductionInfo.Add(productionInfo);

            return productionInfo;
        }



        #endregion ProductionInfo


        #region Drug

        public class DrugProperty
        {
            public TradeName TradeName { get; set; }
            public FormProduct FormProduct { get; set; }
            public INNGroup INNGroup { get; set; }
            public DosageGroup DosageGroup { get; set; }
            public bool IsNew { get; set; }
            public int? ConsumerPackingCount { get; set; }
            public long DrugTypeId { get; set; }
            public bool UseShortDescription { get; set; }
            public Equipment Equipment { get; set; }
            public bool INNGroupNew { get; set; }
        }


        private DrugProperty _currentDrugProperty;
        private ClassifierEditorModelJson _currentModel;

        // Получить все характеристики для Drug
        public DrugProperty GetDrugProperty(ClassifierEditorModelJson model)
        {

            // Если модель уже искалась, то возвращаем результат
            if (_currentModel == model && _currentDrugProperty != null)
                return _currentDrugProperty;
            else
                _currentModel = model;

            // Ищем Equipment
            var equipment = FindOrCreateEquipment(model);

            // Ищем tradeName
            var tradeName = FindOrCreateTradeName(model);

            if (tradeName == null)
                throw new ApplicationException("ТН не может быть пустым");

            // Форма продукта
            var formProduct = FindOrCreateFormProduct(model);

            if (formProduct == null)
                throw new ApplicationException("форма выпуска должна быть указана");

            // Ищем innGroup
            var innGroup = FindOrCreateInnGroup(model);

            // Начинаем искать
            DosageGroup dosageGroup = FindDosageGroup(model) ?? CreateDosageGroup(model);

            // Прямые признаки того, что Drug будет новым
            //bool isNew = tradeName.Id == 0 || (innGroup != null && innGroup.Id == 0) || formProduct.Id == 0 || (dosageGroup != null && dosageGroup.Id == 0) || (equipment != null && equipment.Id == 0);
            bool isNew = tradeName?.Id == 0 || (innGroup?.Id == 0) || formProduct?.Id == 0 || (dosageGroup?.Id == 0) || (equipment?.Id == 0);

            var drugPropery = new DrugProperty
            {
                TradeName = tradeName,
                FormProduct = formProduct,
                INNGroup = innGroup,
                DosageGroup = dosageGroup,
                ConsumerPackingCount = model.ConsumerPackingCount,
                UseShortDescription = model.UseShortDescription,
                DrugTypeId = model.DrugType.Id,
                Equipment = equipment,
                IsNew = isNew,
                INNGroupNew = innGroup != null && innGroup.Id == 0
            };

            _currentDrugProperty = drugPropery;

            return _currentDrugProperty;
        }

        //Получаем существующий или новый Drug
        private Drug GetDrug(ClassifierEditorModelJson model)
        {
            var drugProperty = GetDrugProperty(model);

            ///Ищем Drug
            Drug drug = null;

            // Ищем если
            if (drugProperty.IsNew)
                drug = CreateDrug(model);
            else
                drug = FindDrug(model);


            if (drug == null)
                drug = CreateDrug(model);

            return drug;
        }

        public Drug CheckDrug(ClassifierEditorModelJson model)
        {
            var drugProperty = GetDrugProperty(model);

            if (drugProperty.IsNew)
                return null;

            var drug = FindDrug(model);

            return drug;
        }

        public Drug CreateDrug(ClassifierEditorModelJson model)
        {

            DrugProperty property = GetDrugProperty(model);

            Drug drug = new Drug
            {
                FormProduct = property.FormProduct,
                DosageGroup = property.DosageGroup,
                DrugTypeId = property.DrugTypeId,
                ConsumerPackingCount = property.ConsumerPackingCount,
                UseShortDescription = property.UseShortDescription,
                TradeName = property.TradeName,
                INNGroup = property.INNGroup,
                Equipment = property.Equipment
            };

            _context.Drugs.Add(drug);

            return drug;
        }


        public Drug FindDrug(ClassifierEditorModelJson model)
        {

            DrugProperty property = GetDrugProperty(model);


            var drugs = _context.Drugs.Where(d => d.FormProductId == property.FormProduct.Id &&
                                                      d.TradeNameId == property.TradeName.Id &&
                                                      d.DrugTypeId == property.DrugTypeId);

            if (property.Equipment != null)
                drugs = drugs.Where(d => d.EquipmentId == property.Equipment.Id);
            else
                drugs = drugs.Where(d => d.EquipmentId == null);

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
            {
                //return foundDrugs.First();
                throw new ApplicationException("нарушение целостности Drug");
            }

            if (foundDrugs.Count == 1)
                return foundDrugs.First();

            return null;
        }

        #endregion Drug

    }
}
