using DataAggregator.Core.Filter;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GoodsSystematization
{
    public class GoodsClassifierController : BaseController
    {
        [HttpPost]
        public JsonResult GetClassifier(GoodsClassifierFilter filter, List<long> goodsCategoryIdList, int rettype)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                IQueryable<GoodsSystematizationView> result = goodsCategoryIdList == null || goodsCategoryIdList.Count == 0 ?
                    context.GoodsSystematizationView.Select(sv => sv) :
                    context.GoodsSystematizationView.Where(sv => goodsCategoryIdList.Contains((long)sv.GoodsCategoryId));

                if (!string.IsNullOrEmpty(filter.GoodsTradeName))
                {
                    if (filter.GoodsTradeNameId.HasValue)
                        result = result.Where(sv => sv.GoodsTradeNameId == filter.GoodsTradeNameId);
                    else
                    {
                        List<long> idList = GetIdsForValue(filter.GoodsTradeName, context.GoodsTradeName.Select(s => s));
                        result = result.Where(sv => (idList.Contains(sv.GoodsTradeNameId)));
                    }
                }
                if (filter.Used == false)
                {
                    result = result.Where(sv => sv.Used == true);
                }
                if (!string.IsNullOrEmpty(filter.GoodsDescription))
                {
                    if (filter.GoodsDescriptionId.HasValue)
                        result = result.Where(sv => sv.GoodsId == filter.GoodsDescriptionId);
                    else
                    {
                        string searchMask = filter.GoodsDescription.Replace('*', '%');
                        List<long> idList = context.Goods.Where(x => SqlFunctions.PatIndex(searchMask, x.GoodsDescription) > 0).Select(x => x.Id).ToList();
                        result = result.Where(sv => (idList.Contains(sv.GoodsId)));
                    }
                }

                if (!string.IsNullOrEmpty(filter.OwnerTradeMark))
                {
                    if (filter.OwnerTradeMarkId.HasValue)
                        result = result.Where(sv => sv.OwnerTradeMarkId == filter.OwnerTradeMarkId);
                    else
                    {
                        List<long> idList = GetIdsForValue(filter.OwnerTradeMark, context.Manufacturer.Select(s => s));
                        result = result.Where(sv => idList.Contains(sv.OwnerTradeMarkId));
                    }
                }

                if (!string.IsNullOrEmpty(filter.Packer))
                {
                    if (filter.PackerId.HasValue)
                        result = result.Where(sv => sv.PackerId == filter.PackerId);
                    else
                    {
                        List<long> idList = GetIdsForValue(filter.Packer, context.Manufacturer.Select(s => s));
                        result = result.Where(sv => idList.Contains(sv.PackerId));
                    }
                }

                if (filter.GoodsId.HasValue)
                    result = result.Where(sv => sv.GoodsId == filter.GoodsId);

                if (filter.OwnerTradeMarkId > 0)
                {
                    var idList = context.Manufacturer.Where(x => x.Id == filter.OwnerTradeMarkId).Select(x => x.Id).ToList();

                    result = result.Where(sv => (idList.Contains(sv.OwnerTradeMarkId)));
                }

                if (filter.PackerId > 0)
                {
                    var idList = context.Manufacturer.Where(x => x.Id == filter.PackerId).Select(x => x.Id).ToList();

                    result = result.Where(sv => (idList.Contains(sv.PackerId)));
                }

                if (filter.ToRetail == true)
                {
                    result = result.Where(sv => sv.ToRetail == true);
                }

                if (rettype == 1)
                {
                    var ret_2 = result.Select(s => new SystematizationView_LPDOP()
                    {
                        DrugDescription = s.GoodsDescription,
                        ClassifierId = s.ClassifierId,
                        DrugId = null,
                        GoodsId = s.GoodsId,
                        INNGroup = s.GoodsCategoryName,
                        IsOther = true,
                        OwnerTradeMark = s.OwnerTradeMark,
                        OwnerTradeMarkId = s.OwnerTradeMarkId,
                        Packer = s.Packer,
                        PackerId = s.PackerId,
                        TradeName = s.GoodsTradeName,
                        ConsumerPackingCount = 0,
                        RealPackingCount = 0,
                        RegistrationCertificateNumber = "",
                        Comment = s.Comment,
                        Used = s.Used,
                        ToRetail = s.ToRetail
                    });
                    return Json(ret_2.ToList());
                }
                return Json(result.ToList());
            }
        }

        [HttpPost]
        public JsonResult GetClassifierFromHotKey(string value, List<long> goodsCategoryIdList, int rettype)
        {
            value = value.Trim();
            using (var context = new DrugClassifierContext(APP))
            {
                IQueryable<GoodsSystematizationView> result = goodsCategoryIdList == null || goodsCategoryIdList.Count == 0 ?
    context.GoodsSystematizationView.Select(sv => sv) :
    context.GoodsSystematizationView.Where(sv => goodsCategoryIdList.Contains((long)sv.GoodsCategoryId));
                //   IQueryable<GoodsSystematizationView> result = context.GoodsSystematizationView.Select(sv => sv);

                var tradeNameIdList =
                    context.GoodsTradeName.Where(tn => tn.Value.Contains(value)).Select(tn => tn.Id).ToList();

                result = result.Where(sv => tradeNameIdList.Contains(sv.GoodsTradeNameId) || sv.GoodsDescription.Contains(value));
                if (rettype == 1)
                {
                    var ret_2 = result.Select(s => new SystematizationView_LPDOP()
                    {
                        DrugDescription = s.GoodsDescription,
                        DrugId = null,
                        ClassifierId = s.ClassifierId,
                        GoodsId = s.GoodsId,
                        INNGroup = s.GoodsCategoryName,
                        IsOther = true,
                        OwnerTradeMark = s.OwnerTradeMark,
                        OwnerTradeMarkId = s.OwnerTradeMarkId,
                        Packer = s.Packer,
                        PackerId = s.PackerId,
                        TradeName = s.GoodsTradeName,
                        GoodsCategoryId = (byte)s.GoodsCategoryId,
                        ConsumerPackingCount = 0,
                        RealPackingCount = 0,
                        RegistrationCertificateNumber = "",
                        Comment = s.Comment,
                        Used = s.Used
                    });
                    return Json(ret_2.ToList());
                }
                return Json(result.ToList());
            }
        }

        [HttpPost]
        public JsonResult SetClassifierToGoods(ClassifierToGoodsJson parameters)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                if (!CheckGoodsRelation(context, parameters))
                    return Json(false);

                List<GoodsClassifierInWork> goodsInWork = GetGoodsInWork(parameters.GoodsInWorkIdList, context);

                foreach (var currentGoodsInWork in goodsInWork)
                {
                    currentGoodsInWork.GoodsId = parameters.GoodsId;
                    currentGoodsInWork.OwnerTradeMarkId = parameters.OwnerTradeMarkId;
                    currentGoodsInWork.PackerId = parameters.PackerId;
                    currentGoodsInWork.ClearFlags();
                    currentGoodsInWork.HasChanges = true;

                    var goodsProductionInfo =
                        context.GoodsProductionInfo.SingleOrDefault(
                            p => p.GoodsId == parameters.GoodsId &&
                                 p.OwnerTradeMarkId == parameters.OwnerTradeMarkId &&
                                 p.PackerId == parameters.PackerId);
                    if (goodsProductionInfo != null)
                    {
                        currentGoodsInWork.GoodsCategoryId = goodsProductionInfo.GoodsCategoryId;
                    }
                }

                context.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult ClearClassifierToGoods(List<long> goodsInWorkIdList)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                List<GoodsClassifierInWork> goodsInWork = GetGoodsInWork(goodsInWorkIdList, context);

                foreach (var currentGoodsInWork in goodsInWork)
                {
                    currentGoodsInWork.ClearGoodsId();
                    currentGoodsInWork.HasChanges = true;
                }

                context.SaveChanges();

                return Json(true);
            }
        }

        [HttpPost]
        public JsonResult ChangeGoodsCategory(GoodsCategoryJson parameters)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var goodsClassifierInWork =
                    context.GoodsClassifierInWork.Where(g => parameters.GoodsInWorkIdList.Contains(g.GoodsClearId))
                        .ToList();

                foreach (var item in goodsClassifierInWork)
                {
                    item.GoodsCategoryId = parameters.GoodsCategoryId;
                    item.HasChanges = true;
                }

                context.SaveChanges();

                return Json(true);
            }
        }

        private List<GoodsClassifierInWork> GetGoodsInWork(IList<long> goodsInWorkIdList, DrugClassifierContext context)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            var goodsInWork = context.GoodsClassifierInWork
                .Where(gcw => gcw.UserId == userGuid && goodsInWorkIdList.Contains(gcw.GoodsClearId))
                .ToList();

            if (goodsInWork.Count != goodsInWorkIdList.Count)
                throw new ApplicationException("some goods not found");
            return goodsInWork;
        }

        private static bool CheckGoodsRelation(DrugClassifierContext context, ClassifierToGoodsJson parameters)
        {
            return context.GoodsProductionInfo.Any(d => d.GoodsId == parameters.GoodsId && d.OwnerTradeMarkId == parameters.OwnerTradeMarkId && d.PackerId == parameters.PackerId);
        }

        private static List<long> GetIdsForValue(string value, IQueryable<Domain.Model.Common.DictionaryItem> dictionaryItems)
        {
            string searchMask = value.Replace('*', '%');
            return dictionaryItems.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
        }
    }
}