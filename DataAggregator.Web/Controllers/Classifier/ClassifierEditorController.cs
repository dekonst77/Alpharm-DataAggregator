using DataAggregator.Core.Classifier;
using DataAggregator.Core.Filter;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Domain.Utils;
using DataAggregator.Web.Models.Classifier;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    [Authorize(Roles = "SBoss")]
    public class ClassifierEditorController : BaseController
    {
        private DrugClassifierContext _context;

        private static readonly object LockObject = new object();

        private List<ClassifierPackingBlisterBlockJson> GetClassifierPackingsWithBlister(long ClassifierId)
        {
            var result = new List<ClassifierPackingBlisterBlockJson>();

            List<ClassifierPacking> ClassifierPackings = _context.ClassifierPacking.Where(t => t.ClassifierId == ClassifierId).OrderBy(t => t.Id).ToList(); // упаковки без блистеровки
            BlisterBlock blister_block = _context.BlisterBlock.Where(t => t.ClassifierId == ClassifierId).FirstOrDefault(); // ссылка на блистеровочную упаковку

            ClassifierPackings.ForEach(u =>
            {
                result.Add(new ClassifierPackingBlisterBlockJson(u, blister_block?.ClassifierPackingId == u.Id));
            });

            return result;
        }

        public object r { get; private set; }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~ClassifierEditorController()
        {
            _context.Dispose();
        }



        [HttpPost]
        public ActionResult LoadNfcList(long? formProductId, long? currentNfcId, string formProductValue)
        {
            if (formProductId == null)
            {
                var formProduct = _context.FormProducts.SingleOrDefault(f => f.Value.Equals(formProductValue));

                formProductId = formProduct != null ? (long?)formProduct.Id : null;
            }

            var drugIds = _context.Drugs.Where(d => d.FormProductId == formProductId).Select(d => d.Id).ToList();

            var nfcMatchIds =
                _context.DrugClassification.Where(dc => drugIds.Contains(dc.DrugId)).Select(dc => dc.NFCId).Distinct().ToList();

            var nfcMatchList =
                _context.NFC.Where(nfc => nfcMatchIds.Contains(nfc.Id))
                    .OrderBy(nfc => nfc.Value)
                    .Select(nfc => new { Id = nfc.Id, DisplayValue = "(" + nfc.Value + ") " + nfc.Description })
                    .ToList();

            bool shouldClearNfc = true;

            if (formProductId != null)
            {
                if (currentNfcId != null)
                {
                    shouldClearNfc = !nfcMatchList.Select(n => n.Id).Contains((long)currentNfcId);
                }
                else
                {
                    shouldClearNfc = false;
                }
            }

            nfcMatchList.Add(new { Id = 0L, DisplayValue = "----------------------------------" });

            var nfcDontMatchList =
                _context.NFC.Where(nfc => !nfcMatchIds.Contains(nfc.Id))
                    .OrderBy(nfc => nfc.Value)
                    .Select(nfc => new { Id = nfc.Id, DisplayValue = "(" + nfc.Value + ") " + nfc.Description })
                    .ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    NfcList = nfcMatchList.Concat(nfcDontMatchList),
                    ShouldClearNfc = shouldClearNfc
                }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadATCWhoList(List<INNGroupDosageJson> InnGroupDosage, long? currentATCWhoId, string formProductValue)
        {

            var ATCWhoMatchIds = _context.DrugClassification.Where(w => w.ATCWhoId > 0);

            if (InnGroupDosage != null)
            {
                var inn_Group = _context.INNGroups.Select(s => s.Id).ToList();
                foreach (var inn in InnGroupDosage)
                {
                    var inn_g = _context.INNGroup_INN.Where(w => w.INN.Value == inn.INN.Value).Select(s => s.INNGroupId).ToList();
                    inn_Group = inn_Group.Where(w => inn_g.Contains(w)).ToList();
                }
                var drugIds_inn = _context.Drugs.Where(d => inn_Group.Contains((long)d.INNGroupId)).Select(s => s.Id);
                ATCWhoMatchIds = ATCWhoMatchIds.Where(w => drugIds_inn.Contains(w.DrugId));
            }
            if (!string.IsNullOrEmpty(formProductValue))
            {
                var drugIds_form = _context.Drugs.Where(d => d.FormProduct.Value == formProductValue).Select(s => s.Id);
                ATCWhoMatchIds = ATCWhoMatchIds.Where(w => drugIds_form.Contains(w.DrugId));
            }

            var ATCWhoMatchList =
                _context.ATCWho.Where(ATCWho => ATCWhoMatchIds.Select(s => s.ATCWhoId).Contains(ATCWho.Id) && ATCWho.IsUse == true)
                    .OrderBy(ATCWho => ATCWho.Value)
                    .Select(ATCWho => new { ATCWho.Id, Value = "(" + ATCWho.Value + ") " + ATCWho.Description, ATCWho.IsUse })
                    .ToList();

            bool shouldClearATCWho = true;


            if (currentATCWhoId != null)
            {
                shouldClearATCWho = !ATCWhoMatchList.Select(n => n.Id).Contains((long)currentATCWhoId);
            }
            else
            {
                shouldClearATCWho = false;
            }
            //    }

            ATCWhoMatchList.Add(new { Id = 0L, Value = "----------------------------------", IsUse = true });

            var ATCWhoDontMatchList =
                _context.ATCWho.Where(ATCWho => !ATCWhoMatchIds.Select(s => s.ATCWhoId).Contains(ATCWho.Id) && ATCWho.IsUse == true)
                    .OrderBy(ATCWho => ATCWho.Value)
                    .Select(ATCWho => new { Id = ATCWho.Id, Value = "(" + ATCWho.Value + ") " + ATCWho.Description, IsUse = ATCWho.IsUse })
                    .ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    ATCWhoList = ATCWhoMatchList.Concat(ATCWhoDontMatchList),
                    ShouldClearATCWho = shouldClearATCWho
                }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadGenericList(ClassifierEditorModelJson model)
        {

            if (model.DrugId == 0)
            {
                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new
                    {
                        GenericList = new List<Generic>()
                    }
                };

            }

            model.Clear();

            long? InnGroupId = null;
            bool InnGroupNew = false;
            long? DrugTypeId = null;

            try
            {
                var drugProperty = ClassifierEditor.GetDrugProperty(model);

                InnGroupId = drugProperty.INNGroup?.Id;
                InnGroupNew = drugProperty.INNGroupNew;
                DrugTypeId = drugProperty.DrugTypeId;
            }
            catch
            {
                InnGroupId = null;
                InnGroupNew = false;
                DrugTypeId = null;
            }

            List<Generic> genericList = new List<Generic>();

            string toolTip = String.Empty;

            if (InnGroupId != null)
            {





                //Есди у МНН групп общая, то предлагать только её
                if (_context.ClassificationGeneric.Any(g => g.INNGroupId == InnGroupId && g.GenericId == 5))
                {
                    //Общая
                    genericList.Add(_context.Generic.First(g => g.Id == 5));
                    toolTip = "у «МНН групп» стоит признак: общая группа";
                }
                else
                {
                    if (DrugTypeId == 2)
                    {
                        genericList.Add(_context.Generic.First(g => g.Id == 5));
                        genericList.Add(new Generic() { Id = 0L, Value = "----------------------------------" });
                        toolTip = "Бад";
                    }
                    else if (InnGroupNew)
                    {
                        // Если «МНН групп» новая, то предлагать "оригинальная"
                        genericList.Add(_context.Generic.First(g => g.Id == 1));
                        genericList.Add(new Generic() { Id = 0L, Value = "----------------------------------" });
                        toolTip = "Новая МНН групп";
                    }
                    else
                    {
                        var tooltipList = _context.ClassificationGeneric.Where(d => d.INNGroupId == InnGroupId && d.GenericId == 1).Select(d => d.TradeName.Value).ToList();
                        if (tooltipList != null)
                            toolTip = "Оригинальные : " + String.Join(",", tooltipList);
                    }



                    //Добавляем весь оставшийся список
                    var ids = genericList.Select(b => b.Id).ToList();
                    var otherGenerics = _context.Generic.Where(g => !ids.Contains(g.Id)).OrderBy(g => g.Id).ToList();

                    genericList.AddRange(otherGenerics);
                }
            }


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    GenericList = genericList,
                    toolTip = toolTip
                }
            };

            return jsonNetResult;


        }

        [HttpPost]
        public ActionResult LoadATCEphmraList(List<INNGroupDosageJson> InnGroupDosage, long? currentATCEphmraId, string formProductValue)
        {
            var ATCEphmraMatchIds = _context.DrugClassification.Where(w => w.ATCEphmraId > 0);

            if (InnGroupDosage != null)
            {
                var inn_Group = _context.INNGroups.Select(s => s.Id).ToList();
                foreach (var inn in InnGroupDosage)
                {
                    var inn_g = _context.INNGroup_INN.Where(w => w.INN.Value == inn.INN.Value).Select(s => s.INNGroupId).ToList();
                    inn_Group = inn_Group.Where(w => inn_g.Contains(w)).ToList();
                }
                var drugIds_inn = _context.Drugs.Where(d => inn_Group.Contains((long)d.INNGroupId)).Select(s => s.Id);
                ATCEphmraMatchIds = ATCEphmraMatchIds.Where(w => drugIds_inn.Contains(w.DrugId));
            }
            if (!string.IsNullOrEmpty(formProductValue))
            {
                var drugIds_form = _context.Drugs.Where(d => d.FormProduct.Value == formProductValue).Select(s => s.Id);
                ATCEphmraMatchIds = ATCEphmraMatchIds.Where(w => drugIds_form.Contains(w.DrugId));
            }

            var ATCEphmraMatchList =
                _context.ATCEphmra.Where(ATCEphmra => ATCEphmraMatchIds.Select(s => s.ATCEphmraId).Contains(ATCEphmra.Id) && ATCEphmra.IsUse == true)
                    .OrderBy(ATCWho => ATCWho.Value)
                    .Select(ATCWho => new { Id = ATCWho.Id, Value = "(" + ATCWho.Value + ") " + ATCWho.Description, IsUse = ATCWho.IsUse })
                    .ToList();

            bool shouldClearATCEphmra = true;


            if (currentATCEphmraId != null)
            {
                shouldClearATCEphmra = !ATCEphmraMatchList.Select(n => n.Id).Contains((long)currentATCEphmraId);
            }
            else
            {
                shouldClearATCEphmra = false;
            }


            ATCEphmraMatchList.Add(new { Id = 0L, Value = "----------------------------------", IsUse = true });

            var ATCEphmraDontMatchList =
                _context.ATCEphmra.Where(ATCEphmra => !ATCEphmraMatchIds.Select(s => s.ATCEphmraId).Contains(ATCEphmra.Id) && ATCEphmra.IsUse == true)
                    .OrderBy(ATCEphmra => ATCEphmra.Value)
                    .Select(ATCEphmra => new { Id = ATCEphmra.Id, Value = "(" + ATCEphmra.Value + ") " + ATCEphmra.Description, IsUse = ATCEphmra.IsUse })
                    .ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    ATCEphmraList = ATCEphmraMatchList.Concat(ATCEphmraDontMatchList),
                    ShouldClearATCEphmra = shouldClearATCEphmra
                }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadATCBaaList(List<INNGroupDosageJson> InnGroupDosage, long? currentATCBaaId, string formProductValue)
        {
            var ATCBaaMatchList =
                _context.ATCBaa
                    .OrderBy(ATCBaa => ATCBaa.Value)
                    .Select(ATCBaa => new { ATCBaa.Id, Value = "(" + ATCBaa.Value.Trim() + ") " + ATCBaa.Description.Trim() })
                    .ToList();

            bool shouldClearATCBaa = true;


            if (currentATCBaaId != null)
            {
                shouldClearATCBaa = !ATCBaaMatchList.Select(n => n.Id).Contains((long)currentATCBaaId);
            }
            else
            {
                shouldClearATCBaa = false;
            }


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    ATCBaaList = ATCBaaMatchList,
                    ShouldClearATCBaa = shouldClearATCBaa
                }
            };

            return jsonNetResult;
        }



        [HttpPost]
        public ActionResult LoadFTGList(List<INNGroupDosageJson> InnGroupDosage, long? currentFTGId, string formProductValue)
        {
            var FTGMatchIds = _context.DrugClassification.Where(w => w.FTGId > 0);

            if (InnGroupDosage != null)
            {
                var inn_Group = _context.INNGroups.Select(s => s.Id).ToList();
                foreach (var inn in InnGroupDosage)
                {
                    var inn_g = _context.INNGroup_INN.Where(w => w.INN.Value == inn.INN.Value).Select(s => s.INNGroupId).ToList();
                    inn_Group = inn_Group.Where(w => inn_g.Contains(w)).ToList();
                }
                var drugIds_inn = _context.Drugs.Where(d => inn_Group.Contains((long)d.INNGroupId)).Select(s => s.Id);
                FTGMatchIds = FTGMatchIds.Where(w => drugIds_inn.Contains(w.DrugId));
            }
            if (!string.IsNullOrEmpty(formProductValue))
            {
                var drugIds_form = _context.Drugs.Where(d => d.FormProduct.Value == formProductValue).Select(s => s.Id);
                FTGMatchIds = FTGMatchIds.Where(w => drugIds_form.Contains(w.DrugId));
            }

            var FTGMatchList =
                _context.FTG.Where(ATCWho => FTGMatchIds.Select(s => s.FTGId).Contains(ATCWho.Id))
                    .OrderBy(ATCWho => ATCWho.Value)
                    .Select(ATCWho => new { Id = ATCWho.Id, Value = ATCWho.Value })
                    .ToList();

            bool shouldClearFTG = true;


            if (currentFTGId != null)
            {
                shouldClearFTG = !FTGMatchList.Select(n => n.Id).Contains((long)currentFTGId);
            }
            else
            {
                shouldClearFTG = false;
            }


            FTGMatchList.Add(new { Id = 0L, Value = "----------------------------------" });

            var FTGDontMatchList =
                _context.FTG.Where(FTG => !FTGMatchIds.Select(s => s.FTGId).Contains(FTG.Id))
                    .OrderBy(FTG => FTG.Value)
                    .Select(FTG => new { Id = FTG.Id, Value = FTG.Value })
                    .ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    FTGList = FTGMatchList.Concat(FTGDontMatchList),
                    ShouldClearFTG = shouldClearFTG
                }
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult SearchRegistrationCertificate(RegistrationCertificateFilter filter)
        {
            var results = _context.RegistrationCertificates.Where(r => r.Number.Contains(filter.Number)).ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = results.Select(r => new RegistrationCertificateJson(r))
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadHistory(HistoryFilter filter)
        {

            try
            {
                var history = _context.ClassifierHistoryView.Where(r => //(r.DrugId == filter.DrugId && r.OwnerTradeMarkId == filter.OwnerTradeMarkId && r.PackerId == filter.PackerId) 
                //|| 
                r.ClassifierId == filter.ClassifierId)
                                                            .OrderBy(r => r.When).ToList().Select(ClassifierHistoryModel.Create);


                return ReturnData(history);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPost]
        public ActionResult EditRegistrationCertificate(RegistrationCertificateJson certificate)
        {
            try
            {
                var userGuid = new Guid(User.Identity.GetUserId());
                ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                editor.EditRegistrationCertificate(certificate);

                return ReturnData(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); ;
            }

        }

        [HttpPost]
        public ActionResult LoadClassifier(long ClassifierId)
        {
            try
            {
                var ClassifierInfo = _context.ClassifierInfo.FirstOrDefault(rc => rc.Id == ClassifierId);

                var productionInfo = ClassifierInfo.ProductionInfo;

                var drug = productionInfo.Drug;

                if (drug == null)
                {
                    throw new ApplicationException("drug not found");
                }

                var ownerTradeMark = productionInfo.OwnerTradeMark;

                if (ownerTradeMark == null)
                    throw new ApplicationException("ownerTradeMark not found");

                var packer = productionInfo.Packer;

                if (packer == null)
                    throw new ApplicationException("packer not found");

                var model = new ClassifierEditorModelJson
                {
                    ClassifierId = ClassifierInfo.Id,
                    DrugId = productionInfo.DrugId,
                    OwnerTradeMarkId = productionInfo.OwnerTradeMarkId,
                    PackerId = productionInfo.PackerId,
                    DrugType = new DictionaryJson(drug.DrugType),
                    TradeName = new DictionaryJson(drug.TradeName),
                    FormProduct = new DictionaryJson(drug.FormProduct),
                    ConsumerPackingCount = drug.ConsumerPackingCount,
                    OwnerTradeMark = new DictionaryJson(ownerTradeMark),
                    Packer = new DictionaryJson(packer),
                    ClassifierPackings = GetClassifierPackingsWithBlister(ClassifierId),
                    Equipment = new DictionaryJson(drug.Equipment),
                    InnGroupDosage = new List<INNGroupDosageJson>(),
                    RegistrationCertificate = new RegistrationCertificateJson(),
                    WithoutRegistrationCertificate = productionInfo.WithoutRegistrationCertificate,
                    TotalVolume = new DictionaryJson(drug.DosageGroup != null ? drug.DosageGroup.TotalVolume : null),
                    TotalVolumeCount = drug.DosageGroup != null ? drug.DosageGroup.TotalVolumeCount : null,
                    DosageValue = new DictionaryJson(drug.DosageGroup != null ? drug.DosageGroup.DosageValue : null),
                    DosageValueCount = drug.DosageGroup != null ? drug.DosageGroup.DosageValueCount : null,
                    RealPackingList = new List<RealPackingCountJson>(),
                    ProductionInfoId = productionInfo.Id,
                    IsCompound = drug.INNGroup != null && drug.INNGroup.IsCompound,
                    IsCompoundBAA = drug.INNGroup != null && drug.INNGroup.IsCompoundBAA,
                    UseShortDescription = drug.UseShortDescription,
                    Used = productionInfo.Used,
                    Comment = productionInfo.Comment,
                    Price = null,
                    PriceSourceId = 0,
                    ProductionStage = new DictionaryJson(productionInfo.ProductionStage),
                    ProductionLocalization = new DictionaryJson(productionInfo.ProductionLocalization),
                    PackerLocalization = new DictionaryJson(productionInfo.Packer.Country.Localization)
                };

                if (productionInfo != null)
                {
                    if (productionInfo.PriceVED != null)
                    {
                        model.Price = String.Format(productionInfo.PriceVED % 1 == 0 ? "{0:0}" : "{0:0.00}", productionInfo.PriceVED);
                        model.PriceSourceId = 1;
                    }
                    else if (productionInfo.PriceEtalon != null)
                    {
                        model.Price = String.Format(productionInfo.PriceEtalon % 1 == 0 ? "{0:0}" : "{0:0.00}", productionInfo.PriceEtalon);
                        model.PriceSourceId = 2;
                    }

                    var drugClassification =
                        _context.DrugClassification
                            .FirstOrDefault(dc => dc.DrugId == productionInfo.DrugId && dc.OwnerTradeMarkId == productionInfo.OwnerTradeMarkId);


                    if (drugClassification != null)
                    {
                        if (drugClassification.NFCId.HasValue)
                            model.Nfc = new DictionaryJson() { Id = drugClassification.NFCId.Value };
                        else
                            model.Nfc = new DictionaryJson() { Id = 0 };

                        if (drugClassification.ATCWhoId.HasValue)
                            model.ATCWho = new DictionaryJson()
                            {
                                Id = drugClassification.ATCWho.Id,
                                Value = "(" + drugClassification.ATCWho.Value + ") " +
                                        drugClassification.ATCWho.Description
                            };
                        else
                            model.ATCWho = new DictionaryJson() { Id = 0 };

                        if (drugClassification.ATCEphmraId.HasValue)
                            model.ATCEphmra = new DictionaryJson()
                            {
                                Id = drugClassification.ATCEphmra.Id,
                                Value = "(" + drugClassification.ATCEphmra.Value + ") " +
                                        drugClassification.ATCEphmra.Description
                            };

                        else
                            model.ATCEphmra = new DictionaryJson() { Id = 0 };

                        if (drugClassification.ATCBaaId.HasValue)
                            model.ATCBaa = new DictionaryJson()
                            {
                                Id = drugClassification.ATCBaa.Id,
                                Value = "(" + drugClassification.ATCBaa.Value + ") " +
                                        drugClassification.ATCBaa.Description
                            };
                        else
                            model.ATCBaa = new DictionaryJson() { Id = 0 };

                        if (drugClassification.FTGId.HasValue)
                            model.FTG = new DictionaryJson(drugClassification.FTG);
                        else
                            model.FTG = new DictionaryJson() { Id = 0 };

                        model.IsOtc = drugClassification.IsOtc;
                        model.IsRx = drugClassification.IsRx;
                        model.IsExchangeable = drugClassification.IsExchangeable;
                        model.IsReference = drugClassification.IsReference;

                        if (drugClassification.BrandId.HasValue)
                            model.Brand = new DictionaryJson() { Id = drugClassification.BrandId.Value, Value = drugClassification.Brand.Value };
                        else
                            model.Brand = new DictionaryJson() { Id = 0 };
                    }

                    var classificationGeneric =
                        _context.ClassificationGeneric.FirstOrDefault(
                            cg => cg.TradeNameId == drug.TradeNameId &&
                                  cg.INNGroupId == drug.INNGroupId &&
                                  cg.OwnerTradeMarkId == productionInfo.OwnerTradeMarkId);

                    if (classificationGeneric != null && classificationGeneric.GenericId.HasValue)
                    {
                        model.Generic = new DictionaryJson() { Id = classificationGeneric.GenericId.Value };
                    }
                    else
                    {
                        model.Generic = new DictionaryJson() { Id = 0 };
                    }
                }

                if (drug.DosageGroup != null)
                {
                    model.DosageGroupDescription = drug.DosageGroup.Description;
                    model.ShortDosageGroupDescription = drug.DosageGroup.ShortDescription;
                }

                if (drug.INNGroup != null)
                {
                    model.InnGroupDosageDescription = drug.INNGroup.Description;
                }

                var realPackingList = _context.RealPacking.Where(r => r.DrugId == productionInfo.DrugId).ToList();

                foreach (var r in realPackingList)
                {
                    model.RealPackingList.Add(new RealPackingCountJson()
                    {
                        Id = r.Id,
                        RealPackingCount = r.RealPackingCount
                    });
                }

                if (drug.INNGroup != null && drug.INNGroup.INNGroup_INN != null)
                {
                    foreach (var innLink in drug.INNGroup.INNGroup_INN.OrderBy(i => i.Order))
                    {
                        INNGroupDosageJson inn = new INNGroupDosageJson { INN = new DictionaryJson(innLink.INN) };

                        if (drug.DosageGroup != null && drug.DosageGroup.INNDosages != null)
                        {
                            var dosage = drug.DosageGroup.INNDosages.FirstOrDefault(i => i.Order == innLink.Order);

                            if (dosage != null)
                            {
                                inn.Dosage = new DictionaryJson(dosage.Dosage);
                                inn.DosageCount = dosage.DosageCount;
                            }
                            else
                            {

                                inn.Dosage = new DictionaryJson();
                                inn.DosageCount = String.Empty;
                            }

                        }
                        model.InnGroupDosage.Add(inn);
                    }
                }

                var regCerts = productionInfo.RegistrationCertificate;
                if (regCerts != null)
                {
                    model.RegistrationCertificate = new RegistrationCertificateJson(regCerts);

                    var regClassification = _context.RegistrationCertificateClassification.FirstOrDefault(r => r.RegistrationCertificateId == regCerts.Id);

                    if (regClassification != null)
                    {
                        model.RegistrationCertificate.StorageLife = regClassification.StorageLife;
                    }
                }

                model.Restore();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = model
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult GetClassifierEditorView(ClassifierFilter filter)
        {
            try
            {
                string xmlFilter = XmlToObjectSerializer<ClassifierFilter>.Serialize(filter);

                IEnumerable<ClassifierEditorFilterView> result_list = _context.GetClassifierEditorView_Result(xmlFilter).ToList();
                return ReturnData(result_list);                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult CheckClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    var data = editor.CheckClassifier(model);

                    return ReturnData(data);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult TryAddClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    var data = editor.AddClassifier(model, true);

                    return ReturnData(data);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult AddClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    var data = editor.AddClassifier(model, false);

                    var itemView = _context.ClassifierEditorFilterView.SingleOrDefault(r => r.DrugId == data.DrugId &&
                        r.OwnerTradeMarkId == data.OwnerTradeMarkId &&
                        r.PackerId == data.PackerId);

                    var result = new { data = data, itemView = itemView };

                    return ReturnData(result);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult GetChanges(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    var data = editor.GetChanges(model);

                    return ReturnData(data);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        public ActionResult CheckRecreate(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);

                    var data = editor.CheckRecreate(model);

                    return ReturnData(data);

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult ChangeUse(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    editor.ChangeUse(model);
                    return ReturnData(null);
                }
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult ChangeClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);
                    var data = editor.ChangeClassifier(model, true);
                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult ReCreateClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);

                    var data = editor.ReCreateClassifier(model);

                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult MergeClassifier(ClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    ClassifierEditor editor = new ClassifierEditor(_context, userGuid);

                    var data = editor.MergeClassifier(model, true);

                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }


        private ActionResult ReturnError(Exception e)
        {
            ResponseModel result = new ResponseModel();

            result.Success = false;
            result.Data = null;
            result.ErrorMessage = e.Message;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            LogError(e);

            return jsonNetResult;
        }


        private class ResponseModel
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public object Data { get; set; }
        }

        private static new ActionResult ReturnData(object data)
        {
            ResponseModel result = new ResponseModel();

            result.Success = true;
            result.Data = data;
            result.ErrorMessage = null;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Редактирование упаковок: поле блистеровки
        /// </summary>
        /// <param name="id">ID упаковки</param>
        /// <param name="ClassifierId">ID классификатора</param>
        /// <param name="newValue">наличие или отсутствие ссылки на ID упаковки</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClassifierPackingEdit(int id, long ClassifierId, bool newValue)
        {
            try
            {
                int? NewClassifierPackingId;

                if (newValue)
                    NewClassifierPackingId = id;
                else
                    NewClassifierPackingId = null;

                var BlisterEntity = _context.BlisterBlock.Find(ClassifierId);

                if (BlisterEntity == null)
                {
                    BlisterEntity = new BlisterBlock { ClassifierId = ClassifierId, ClassifierPackingId = NewClassifierPackingId };
                    _context.BlisterBlock.Add(BlisterEntity);
                }
                else
                    BlisterEntity.ClassifierPackingId = NewClassifierPackingId;


                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = GetClassifierPackingsWithBlister(ClassifierId)
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}