using DataAggregator.Core.Filter;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Web.Models.Systematization;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    public class ClassifierController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~ClassifierController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public JsonResult GetClassifier(ClassifierFilter filter, int rettype=0)
        {
            var result = _context.SystematizationView.Where(sv => sv.RegistrationCertificateIsBlocked != true);

            if (!string.IsNullOrEmpty(filter.TradeName))
            {
                if (filter.TradeNameId != null)
                    result = result.Where(sv => sv.TradeNameId == filter.TradeNameId);
                else
                {
                    var searchMask = filter.TradeName.Replace('*', '%');
                    var idList = _context.TradeNames.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.TradeNameId ?? -1)));
                }
            }

            if (!string.IsNullOrEmpty(filter.RuNumber))
            {
                result = result.Where(sv => sv.RegistrationCertificateNumber.Contains(filter.RuNumber));
            }

            if (!string.IsNullOrEmpty(filter.OwnerTradeMark))
            {
                if (filter.OwnerTradeMarkId != null)
                    result = result.Where(sv => sv.OwnerTradeMarkId == filter.OwnerTradeMarkId);
                else
                {
                    var searchMask = filter.OwnerTradeMark.Replace('*', '%');
                    var idList = _context.Manufacturer.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.OwnerTradeMarkId ?? -1)));
                }
            }

            if (!string.IsNullOrEmpty(filter.INNGroup))
            {
                if (filter.INNGroupId != null)
                    result = result.Where(sv => sv.INNGroupId == filter.INNGroupId);
                else
                {
                    var searchMask = filter.INNGroup.Replace('*', '%');
                    var idList = _context.INNGroups.Where(x => SqlFunctions.PatIndex(searchMask, x.Description) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.INNGroupId ?? -1)));
                }
            }

            if (!string.IsNullOrEmpty(filter.Packer))
            {
                if (filter.PackerId != null)
                    result = result.Where(sv => sv.PackerId == filter.PackerId);
                else
                {
                    var searchMask = filter.Packer.Replace('*', '%');
                    var idList = _context.Manufacturer.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.PackerId ?? -1)));
                }
            }

            if (!string.IsNullOrEmpty(filter.FormProduct))
            {
                if (filter.FormProductId != null)
                    result = result.Where(sv => sv.FormProductId == filter.FormProductId);
                else
                {
                    var searchMask = filter.FormProduct.Replace('*', '%');
                    var idList = _context.FormProducts.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.FormProductId ?? -1)));
                }
            }
            
            if (!string.IsNullOrEmpty(filter.DosageGroup))
            {
                if (filter.DosageGroupId != null)
                    result = result.Where(sv => sv.DosageGroupId == filter.DosageGroupId);
                else
                {
                    var searchMask = filter.DosageGroup.Replace('*', '%');
                    var idList = _context.DosageGroups.Where(x => SqlFunctions.PatIndex(searchMask, x.Description) > 0).Select(x => x.Id).ToList();
                    result = result.Where(sv => (idList.Contains(sv.DosageGroupId ?? -1)));
                }
            }

            if (filter.ConsumerPackingCount != null)
                result = result.Where(sv => sv.ConsumerPackingCount == filter.ConsumerPackingCount);

            if (filter.Used != null)
                result = result.Where(sv => sv.Used == (filter.Used == 1));

            if (filter.DrugId.HasValue)
                result = result.Where(sv => sv.DrugId == filter.DrugId);    

            if (filter.OwnerTradeMarkId>0)
            {
                var idList = _context.Manufacturer.Where(x => x.Id == filter.OwnerTradeMarkId).Select(x => x.Id).ToList();
                result = result.Where(sv => sv.OwnerTradeMarkId== filter.OwnerTradeMarkId);
            }

            if (!string.IsNullOrEmpty(filter.RegNumber))
            {
                result = result.Where(sv => sv.RegistrationCertificateNumber.Contains(filter.RegNumber));
            }

            if (!string.IsNullOrEmpty(filter.Comment))
            {
                result = result.Where(sv => sv.Comment.Contains(filter.Comment));
            }

            if (rettype == 1)
            {
                var ret_2 = result.Select(s => new SystematizationView_LPDOP()
                {
                    DrugDescription = s.DrugDescription,
                    DrugId = s.DrugId,
                    ClassifierId=s.ClassifierId,
                    GoodsId = null,
                    INNGroup = s.INNGroup,
                    IsOther = false,
                    OwnerTradeMark = s.OwnerTradeMark,
                    OwnerTradeMarkId = s.OwnerTradeMarkId,
                    Packer = s.Packer,
                    PackerId = s.PackerId,
                    TradeName = s.TradeName,
                    ConsumerPackingCount = s.ConsumerPackingCount,
                    RealPackingCount = s.RealPackingCount,
                    GoodsCategoryId = null,
                    Used=s.Used,
                    Comment=s.Comment,
                    RegistrationCertificateNumber = s.RegistrationCertificateNumber,
                    Price = s.Price.ToString()
                });
                return Json(ret_2.ToList());
            }
            return Json(result.ToList());
        }
        [HttpPost]
        public JsonResult GetExternalView_FULL(long? ClassifierId, string RuNumber, string TradeName, string DrugDescription, string INN, string OwnerTradeMark, string Packer, bool? Used)
        {
            var result = _context.ExternalView_FULL.Where(w => 1 == 1);

            if (ClassifierId != null)
                result = result.Where(sv => sv.Used == (ClassifierId == (long)ClassifierId));
            if (!string.IsNullOrEmpty(TradeName)&& TradeName== INN)
            {
                result = result.Where(sv => sv.TradeName.Contains(TradeName) || sv.INNGroup.Contains(INN));
            }
            else
            {
                if (!string.IsNullOrEmpty(TradeName))
                {
                    result = result.Where(sv => sv.TradeName.Contains(TradeName));
                }
                if (!string.IsNullOrEmpty(DrugDescription))
                {
                    result = result.Where(sv => sv.DrugDescription.Contains(DrugDescription));
                }

                if (!string.IsNullOrEmpty(RuNumber))
                {
                    result = result.Where(sv => sv.RegistrationCertificateNumber.Contains(RuNumber));
                }

                if (!string.IsNullOrEmpty(OwnerTradeMark))
                {
                    result = result.Where(sv => sv.OwnerTradeMark.Contains(OwnerTradeMark));
                }

                if (!string.IsNullOrEmpty(INN))
                {
                    result = result.Where(sv => sv.INNGroup.Contains(INN));
                }

                if (!string.IsNullOrEmpty(Packer))
                {
                    result = result.Where(sv => sv.Packer.Contains(Packer));
                }
            }
            if (Used != null)
                result = result.Where(sv => sv.Used == (Used == (bool)Used));


            return Json(result.ToList());
        }
        [HttpPost]
        public JsonResult GetClassifierFromHotKey(string value, int rettype=0)
        {
            value = value.Trim();
            var tradeNameIdList = _context.TradeNames.Where(tn => tn.Value.Contains(value)).Select(tn => tn.Id).ToList();
            var innGroupIdList = _context.INNGroups.Where(i => i.Description.Contains(value)).Select(i => i.Id).ToList();

            var result =
                _context.SystematizationView.Where(
                    sv =>
                        (tradeNameIdList.Contains(sv.TradeNameId ?? -1) || innGroupIdList.Contains(sv.INNGroupId ?? -1) || sv.RegistrationCertificateNumber.Contains(value)) &&
                        sv.Used && sv.RegistrationCertificateIsBlocked != true).ToList();
            if (rettype == 1)
            {
                var ret_2 = result.Select(s => new SystematizationView_LPDOP()
                {
                    DrugDescription = s.DrugDescription,
                    DrugId = s.DrugId,
                    ClassifierId=s.ClassifierId,
                    GoodsId = null,
                    INNGroup = s.INNGroup,
                    IsOther = false,
                    OwnerTradeMark = s.OwnerTradeMark,
                    OwnerTradeMarkId = s.OwnerTradeMarkId,
                    Packer = s.Packer,
                    PackerId = s.PackerId,
                    TradeName = s.TradeName,
                    ConsumerPackingCount = s.ConsumerPackingCount,
                    RealPackingCount = s.RealPackingCount,
                    GoodsCategoryId = null,
                    Used = s.Used,
                    Comment=s.Comment,
                    RegistrationCertificateNumber =s.RegistrationCertificateNumber

                });
                return Json(ret_2.ToList());
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult ClearClassifierToDrugs(List<long> drugInWorkIdList)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            var drugsInWork = _context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid && drugInWorkIdList.Contains(dcw.Id)).ToList();

            if (drugInWorkIdList.Count != drugsInWork.Count)
                throw new ApplicationException("drug not found");

            foreach (var drugInWork in _context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid && drugInWorkIdList.Contains(dcw.Id)).ToList())
            {
                drugInWork.ClearDrugId();
                drugInWork.HasChanges = true;
            }

            _context.SaveChanges();

            return Json(true);
        }
        

        [HttpPost]
        public JsonResult SetClassifierToDrugs(ClassifierToDrugsJson parameters)
        {
            var drug_PI = _context.ProductionInfo.SingleOrDefault(d => d.DrugId == parameters.DrugId && d.OwnerTradeMarkId == parameters.OwnerTradeMarkId && d.PackerId == parameters.PackerId);
            var good_PI = _context.GoodsProductionInfo.SingleOrDefault(d => d.GoodsId == parameters.GoodsId && d.OwnerTradeMarkId == parameters.OwnerTradeMarkId && d.PackerId == parameters.PackerId);

            if (parameters.DrugId > 0 && drug_PI==null)
                throw new ApplicationException("Не обнаружен ЛП");

            if (parameters.GoodsId > 0 && good_PI==null)
                throw new ApplicationException("Не обнаружен ДОП");

            if (drug_PI == null && good_PI == null)
                throw new ApplicationException("Не обнаружен смысл");



            var userGuid = new Guid(User.Identity.GetUserId());

            // список DCW для изменения привязки
            var drugsInWork = _context.DrugClassifierInWork.Where(dcw => dcw.UserId == userGuid && parameters.DrugInWorkIdList.Contains(dcw.Id)).ToList();

            if (drugsInWork.Count != parameters.DrugInWorkIdList.Count)
                throw new ApplicationException("some drugs not found");

            foreach (var currentDrugInWork in drugsInWork)
            {
                currentDrugInWork.DrugId = parameters.DrugId;
                currentDrugInWork.GoodsId = parameters.GoodsId;
                if (parameters.DrugId > 0)
                {
                    currentDrugInWork.GoodsId = null;
                    currentDrugInWork.IsOther = false;
                    currentDrugInWork.GoodsCategoryId = null;

                    currentDrugInWork.RealPackingCount = parameters.RealPackingCount;
                    currentDrugInWork.ConsumerPackingCount = drug_PI.Drug.ConsumerPackingCount;
                }
                else
                {
                    currentDrugInWork.DrugId = null;
                    currentDrugInWork.IsOther = true;

                    currentDrugInWork.RealPackingCount = 0;
                    currentDrugInWork.ConsumerPackingCount = 0;

                    //if (goodsProductionInfo != null)
                   // {
                        currentDrugInWork.GoodsCategoryId = (byte)good_PI.GoodsCategoryId;
                   // }
                }

                
                currentDrugInWork.OwnerTradeMarkId = parameters.OwnerTradeMarkId;
                currentDrugInWork.PackerId = parameters.PackerId;
                

                currentDrugInWork.ForChecking = false;
                currentDrugInWork.ForAdding = false;

                currentDrugInWork.HasChanges = true;
            }

            _context.SaveChanges();

            return Json(true);
        }

      /*  private bool CheckDrugRelation(ClassifierToDrugsJson parameters)
        {
            return _context.ProductionInfo.Any(d => d.DrugId == parameters.DrugId && d.OwnerTradeMarkId == parameters.OwnerTradeMarkId && d.PackerId == parameters.PackerId);
        }
        private bool CheckGoodsRelation(ClassifierToDrugsJson parameters)
        {
            return _context.GoodsProductionInfo.Any(d => d.GoodsId == parameters.GoodsId && d.OwnerTradeMarkId == parameters.OwnerTradeMarkId && d.PackerId == parameters.PackerId);
        }*/
    }
}