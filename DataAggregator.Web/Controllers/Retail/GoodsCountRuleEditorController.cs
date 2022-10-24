using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Domain.Model.GoodsData.QueryModel;
using DataAggregator.Web.Models.Retail;
using DataAggregator.Web.Models.Retail.GoodsCountRuleEditor;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class GoodsCountRuleEditorController : BaseController
    {
        /// <summary>
        /// Загрузка правил
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="actualityType">Тип актуальности правил</param>
        [HttpPost]
        public ActionResult LoadRules(int year, int month, ActualityType actualityType)
        {
            return new JsonNetResult(LoadCountRuleModels(year, month, actualityType));
        }

        /// <summary>
        /// Загрузка правил
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="actualityType">Тип актуальности правил</param>
        private List<GoodsCountRuleModel> LoadCountRuleModels(int year, int month, ActualityType actualityType)
        {
            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCountRuleView> data = context.GoodsCountRuleView.
                    Where(cr => cr.Year == year && cr.Month == month)
                    .OrderByDescending(cr => cr.Id)
                    .ToList();

                if (actualityType == ActualityType.Actual)
                    data = data.Where(IsActual).ToList();

                if (actualityType == ActualityType.NotActual)
                    data = data.Where(s => !IsActual(s)).ToList();

                return data.Select(GoodsCountRuleModel.Create).ToList();
            }
        }

        private static bool IsActual(GoodsCountRuleView s)
        {
            return s.ClassifierId != s.DistributionClassifierId;
        }

        [HttpPost]
        public ActionResult SearchGoodsInR12(long brandId, int year, int month, string regionCode)
        {
            if (year > 2000)
                year = year - 2000;

            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCalcRuleModel> goods = context.SearchGoodsInR12CountRuleModel(brandId, year, month, regionCode);

                return CreateGoodsList(goods, true);
            }
        }

        [HttpPost]
        public ActionResult SearchGoodsInR1(long brandId, int year, int month, string regionCode)
        {
            if (year > 2000)
                year = year - 2000;

            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCalcRuleModel> goods = context.SearchGoodsInR1CountRuleModel(brandId, year, month, regionCode);

                return CreateGoodsList(goods, true);
            }
        }

        [HttpPost]
        public ActionResult SearchGoodsInRus(long brandId, int year, int month)
        {
            if (year > 2000)
                year = year - 2000;

            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCalcRuleModel> goods = context.SearchGoodsInRusCountRuleModel(brandId, year, month);

                return CreateGoodsList(goods, true);
            }
        }

        [HttpPost]
        public ActionResult SearchGoods(long brandId, int year, int month)
        {
            if (year > 2000)
                year = year - 2000;

            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCalcRuleModel> goods = context.SearchGoodsInRusCountRuleModel(brandId, year, month);

                return CreateGoodsList(goods, false);
            }
        }

        private static ActionResult CreateGoodsList(List<GoodsCalcRuleModel> goods, bool withSum)
        {
            var result = new List<GoodsCountRuleData>();

            if (goods != null && goods.Count > 0)
            {
                result = goods.Select(r => new GoodsCountRuleData()
                {
                    GoodsId = r.GoodsId,
                    OwnerTradeMarkId = r.OwnerTradeMarkId,
                    PackerId = r.PackerId,
                    BrandId = r.BrandId,
                    Brand = r.Brand,
                    GoodsDescription = string.Format("{0} {1} {2}", r.GoodsDescription, r.OwnerTradeMark, r.Packer),
                    GoodsTradeName = r.GoodsTradeName,
                    OwnerTradeMark = r.OwnerTradeMark,
                    Packer = r.Packer,
                    SellingSumNDS = GetDecimalData(r.SellingSumNDS, withSum),
                    SellingCount = GetDecimalData(r.SellingCount, withSum),
                    SellingSumNDSPart = GetDecimalData(r.SellingSumNDSPart, withSum),
                    SellingCountPart = GetDecimalData(r.SellingCountPart, withSum),
                    IsInCountRules = r.IsInCountRules,
                    ClassifierId = r.ClassifierId
                }).ToList();
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        private static decimal? GetDecimalData(decimal? source, bool withSum)
        {
            return withSum && source != null ? Math.Round((decimal)source, 2) : (decimal?)null;
        }

        [HttpPost]
        public ActionResult DeleteRow(int id)
        {
            using (var context = new GoodsDataContext(APP))
            {
                GoodsCountRule rule = context.GoodsCountRule.Single(cr => cr.Id == id);

                context.GoodsCountRule.Remove(rule);

                context.SaveChanges();

                //Возвращаем успешный результат
                return new JsonNetResult(true);
            }
        }

        /// <summary>
        /// Сохраняем изменения в правиле
        /// </summary>
        [HttpPost]
        public ActionResult SaveRow(GoodsCountRuleModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            try
            {
                GoodsCountRuleModel newModel = ChangeCountRuleModel(model);

                //Возвращаем успешный результат
                return new JsonNetResult(newModel);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        private GoodsCountRuleModel ChangeCountRuleModel(GoodsCountRuleModel model)
        {
            if (model.SellingSumPart > 100 ||
                model.SellingSumPart < 0)
                throw new InvalidOperationException("Процент должен быть от 0 до 100%");

            using (var context = new GoodsDataContext(APP))
            {
                // ищем идентичные правила
                CheckSimilarRule(model, context);

                // ищем зависимые правила
                CheckRelatedRule(model, context);

                GoodsCountRule rule = context.GoodsCountRule.FirstOrDefault(cr => cr.Id == model.Id);

                if (rule == null)
                {
                    rule = new GoodsCountRule();
                    context.GoodsCountRule.Add(rule);
                }

                // Проверка существования средней цены, если переброс идет на конкретный препарат

                if (model.DistributionGoodsId != null && model.DistributionOwnerTradeMarkId != null && model.DistributionPackerId != null)
                {
                    var hasSellingData = context.CheckDataForAvgPrice(
                        model.Year,
                        model.Month,
                        model.RegionCode,
                        model.RegionMsk,
                        model.RegionSpb,
                        model.RegionRus,
                        (long)model.DistributionGoodsId,
                        (long)model.DistributionOwnerTradeMarkId,
                        (long)model.DistributionPackerId
                    );

                    if (model.SellingSumPart > 0 && (!hasSellingData.HasValue || !hasSellingData.Value))
                        throw new InvalidOperationException("Нет средней цены");
                }

                //Сохраняем
                var userGuid = new Guid(User.Identity.GetUserId());

                model.SetGoodsCountRule(rule, userGuid);

                context.SaveChanges();


                GoodsCountRuleView ruleView = context.GoodsCountRuleView.Single(r => r.Id == rule.Id);

                //Возвращаем успешный результат
                return GoodsCountRuleModel.Create(ruleView);
            }
        }

        private static void CheckSimilarRule(GoodsCountRuleModel model, GoodsDataContext context)
        {
            List<long> identicalRulesIdList = context.GoodsCountRule.Where(cr =>
                cr.Id != model.Id &&
                cr.Year == model.Year &&
                cr.Month == model.Month &&
                cr.GoodsId == model.GoodsId && cr.OwnerTradeMarkId == model.OwnerTradeMarkId && cr.PackerId == model.PackerId &&
                cr.DistributionGoodsId == model.DistributionGoodsId && cr.DistributionOwnerTradeMarkId == model.DistributionOwnerTradeMarkId && cr.DistributionPackerId == model.DistributionPackerId &&
                (
                    cr.RegionCode != null && cr.RegionCode == model.RegionCode ||
                    cr.RegionCode.Contains("77.") && model.RegionMsk ||
                    cr.RegionMsk && model.RegionCode.Contains("77.") ||
                    cr.RegionMsk && model.RegionMsk ||
                    cr.RegionCode.Contains("78.") && model.RegionSpb ||
                    cr.RegionSpb && model.RegionCode.Contains("78.") ||
                    cr.RegionSpb && model.RegionSpb ||
                    model.RegionRus ||
                    cr.RegionRus
                )).Select(cr => cr.Id).ToList();

            if (identicalRulesIdList.Any())
                throw new InvalidOperationException(string.Format("Идентичное правило существует ({0})", string.Join(", ", identicalRulesIdList)));
        }

        private static void CheckRelatedRule(GoodsCountRuleModel model, GoodsDataContext context)
        {
            List<long> chainRulesIdList = context.GoodsCountRule.Where(cr =>
                cr.Id != model.Id &&
                cr.Year == model.Year &&
                cr.Month == model.Month &&
                (model.GoodsId != null && cr.DistributionGoodsId == model.GoodsId && cr.DistributionOwnerTradeMarkId == model.OwnerTradeMarkId && cr.DistributionPackerId == model.PackerId ||
                 model.DistributionGoodsId != null && cr.GoodsId == model.DistributionGoodsId && cr.OwnerTradeMarkId == model.DistributionOwnerTradeMarkId && cr.PackerId == model.DistributionPackerId) &&
                (
                    cr.RegionCode != null && model.RegionCode != null && cr.RegionCode == model.RegionCode ||
                    cr.RegionCode.Contains("77.") && model.RegionMsk ||
                    cr.RegionMsk && model.RegionCode.Contains("77.") ||
                    cr.RegionMsk && model.RegionMsk ||
                    cr.RegionCode.Contains("78.") && model.RegionSpb ||
                    cr.RegionSpb && model.RegionCode.Contains("78.") ||
                    cr.RegionSpb && model.RegionSpb ||
                    model.RegionRus ||
                    cr.RegionRus
                )).Select(cr => cr.Id).ToList();

            if (chainRulesIdList.Any())
                throw new InvalidOperationException(string.Format("Есть зависимые правила ({0})", string.Join(", ", chainRulesIdList)));
        }

        public ActionResult ErrorMessage(string errorMessage)
        {
            return new JsonNetResult(new { isError = true, errorMessage });
        }

    }
}