using DataAggregator.Core.Filter;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Stat;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Domain.Model.DrugClassifier.Systematization.View;
using DataAggregator.Web.Models.GoodsSystematization;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GoodsSystematization
{
    [Authorize(Roles = "SBoss, SOperator, SPharmacist")]
    public class GoodsSystematizationController : BaseController
    {
        private static readonly object LockGoodsData = new object();

        [HttpPost]
        public JsonResult GetGoods(GoodsFilterResultJson goodsFilterResult)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
            {
                // у пользователя не должно быть данных в работе
                int userGoodsCount = context.GoodsClassifierInWork.Count(gcw => gcw.UserId == userGuid);
                if (userGoodsCount != 0)
                    throw new ApplicationException("user data not empty");

                var goodsFilter = new GoodsFilter
                {
                    Count = goodsFilterResult.Count,
                    ForWorkCategoryIds = goodsFilterResult.ForWorkCategoryIds,
                    ForAddingCategoryIds = goodsFilterResult.ForAddingCategoryIds,
                    UserGuids = goodsFilterResult.UserGuids,
                    Additional = goodsFilterResult.Additional
                };

                if (goodsFilter.Count > 100000)
                    goodsFilter.Count = 100000;

                string query = goodsFilter.GetFilter();

                if (!string.IsNullOrEmpty(query))
                    lock (LockGoodsData)
                        context.GetGoods(query, userGuid);
            }

            return LoadGoods();
        }

        [HttpPost]
        public JsonResult SetGoods()
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            using (var context = new DrugClassifierContext(APP))
                lock (LockGoodsData)
                    context.SetGoods(userGuid);

            return Json(true);
        }

        [HttpPost]
        public JsonResult LoadGoods()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var userGuid = new Guid(User.Identity.GetUserId());
                List<GoodsInWorkView> goods = context.GoodsInWorkView.Where(d => d.UserId == userGuid).Take(100000).ToList();

                JsonResult jsonResult = Json(goods, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
        }

        [HttpPost]
        public JsonResult LoadGoodsCategories()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var result =
                    context.GoodsCategory.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        SectionName = c.GoodsSection.Name
                    }).ToList();

                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ForAdding(List<long> goodsInWorkIdList, bool value)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                List<GoodsClassifierInWork> goodsInWork = context.GoodsClassifierInWork.Where(gcw => gcw.UserId == userGuid && goodsInWorkIdList.Contains(gcw.GoodsClearId)).ToList();

                if (goodsInWork.Count != goodsInWorkIdList.Count)
                    throw new ApplicationException("some goods not found");

                foreach (GoodsClassifierInWork currentGoodsInWork in goodsInWork)
                {
                    currentGoodsInWork.ForAdding = value;
                    currentGoodsInWork.HasChanges = true;
                    if (value)
                        currentGoodsInWork.ClearGoodsId();
                }

                context.SaveChanges();

                return Json(true);
            }
        }

        [HttpPost]
        [Authorize(Roles = "SBoss")]
        public JsonResult UpdateStatistic()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 3600;
                context.Database.ExecuteSqlCommand("exec Stat.UpdateGoodsStat");

                return GetGoodsFilterStatistic();
            }
        }

        [HttpPost]
        public JsonResult GetGoodsFilterStatistic()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var categoryStat = new List<GoodsCategoryStatJson>();
                foreach (var gcs in context.GoodsCategoryStat.ToList())
                    categoryStat.Add(new GoodsCategoryStatJson(gcs));

                List<GoodsUserStat> userStat = context.GoodsUserStat.ToList();

                var filter = new GoodsFilterJson
                {
                    CategoryStat = categoryStat,
                    UserStat = userStat,
                    Count = 300
                };
                
                return Json(filter);
            }
        }
    }
}