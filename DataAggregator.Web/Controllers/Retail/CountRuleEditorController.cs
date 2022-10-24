using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.QueryModel;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Web.Models.Retail;
using DataAggregator.Web.Models.Retail.CountRuleEditor;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class CountRuleEditorController : BaseController
    {




        #region Исправлено
        [HttpPost]
        public ActionResult SearchDrug(int classifierId, int brandId, int year, int month, string regionCode)
        {


            using (var context = new RetailContext())
            {
                List<CalcRuleModel> drugs = context.SearchDrugCountRuleModel(classifierId, brandId, year, month, regionCode);

                return CreateDrugList(drugs, true);
            }
        }


        private static ActionResult CreateDrugList(List<CalcRuleModel> drugs, bool withSum)
        {
            var result = new List<CountRuleDrugData>();

            if (drugs != null && drugs.Count > 0)
            {
                result = drugs.Select(r => new CountRuleDrugData()
                {
                    BrandId = r.BrandId,
                    Brand = r.Brand,
                    DrugDescription = string.Format("{0} {1} {2}", r.DrugDescription, r.OwnerTradeMark, r.Packer),
                    TradeName = r.TradeName,
                    OwnerTradeMark = r.OwnerTradeMark,
                    Packer = r.Packer,
                    SellingSumNDS = GetDecimalData(r.SellingSumNDS, withSum),
                    PurchaseSumNDS = GetDecimalData(r.PurchaseSumNDS, withSum),
                    SellingCount = GetDecimalData(r.SellingCount, withSum),
                    PurchaseCount = GetDecimalData(r.PurchaseCount, withSum),
                    SellingSumNDSPart = GetDecimalData(r.SellingSumNDSPart, withSum),
                    PurchaseSumNDSPart = GetDecimalData(r.PurchaseSumNDSPart, withSum),
                    SellingCountPart = GetDecimalData(r.SellingCountPart, withSum),
                    PurchaseCountPart = GetDecimalData(r.PurchaseCountPart, withSum),
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

        /// <summary>
        /// Загрузка правил
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="actualityType">Тип актуальности правил</param>
        /// <returns></returns>
        private static List<CountRuleModel> LoadCountRuleModels(int year, int month)
        {
            using (var context = new RetailContext())
            {
                List<CountRuleView> data = context.CountRuleView.
                    Where(cr => cr.Year * 100 + cr.Month <= year * 100 + month &&
                    (!cr.YearEnd.HasValue || (year * 100 + month <= cr.YearEnd * 100 + cr.MonthEnd)))
                    .OrderByDescending(cr => cr.Id)
                    .ToList();

                var usedRule = context.CountRuleUsed.Where(c => c.Year == year && c.Month == month).ToList();

                var rule = data.Select(CountRuleModel.Create).ToList();

                var ruleGr = data.Where(d => d.ClassifierId != null)
                                                     .GroupBy(d => d.ClassifierId)
                                                     .Select(g => new { ClassifierId = g.Key, Count = g.Count() })
                                                     .Where(g=>g.Count > 1)
                                                     .ToList();

                rule.ForEach(r =>
                {
                    var used = usedRule.FirstOrDefault(ur => ur.CountRuleId == r.Id);

                    var doubled = ruleGr.FirstOrDefault(ur => ur.ClassifierId == r.ClassifierId);

                    if (used != null)
                    {
                        r.InUsed = used.InUsed;
                        r.OutUsed = used.OutUsed;
                    }

                    if (doubled != null)
                    {
                        r.Flag = "doubleFrom";
                    }
                });

                return rule;
            }
        }


        /// <summary>
        /// Загрузка правил
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="actualityType">Тип актуальности правил</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadRules(int year, int month)
        {
            return new JsonNetResult(LoadCountRuleModels(year, month));
        }

        [HttpPost]
        public ActionResult DeleteRow(int id)
        {
            using (var context = new RetailContext())
            {
                CountRule rule = context.CountRule.Single(cr => cr.Id == id);

                context.CountRule.Remove(rule);

                context.SaveChanges();

                //Возвращаем успешный результат
                return new JsonNetResult(true);
            }
        }

        /// <summary>
        /// Сохраняем изменения в правиле
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRow(CountRuleModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            try
            {
                CountRuleModel newModel = ChangeCountRuleModel(model);

                //Возвращаем успешный результат
                return new JsonNetResult(newModel);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        private CountRuleModel ChangeCountRuleModel(CountRuleModel model)
        {
            if (model.PurchaseSumPart > 100 ||
                model.SellingSumPart > 100 ||
                model.PurchaseSumPart < 0 ||
                model.SellingSumPart < 0)
            {
                throw new InvalidOperationException("Процент должен быть от 0 до 100%");
            }

            using (var context = new RetailContext())
            {
                // ищем идентичные правила
                CheckSimilarRule(model, context);

                // ищем зависимые правила
                CheckRelatedRule(model, context);

                CountRule rule = context.CountRule.FirstOrDefault(cr => cr.Id == model.Id);

                if (rule == null)
                {
                    rule = new CountRule();
                    context.CountRule.Add(rule);
                }

                // Проверка существования средней цены, если переброс идет на конкретный препарат

                //if (model.DistributionClassifierId != null)
                //{
                //    var checkDataForAvgPrice = context.CheckDataForAvgPrice(
                //        model.Year,
                //        model.Month,
                //        model.RegionCode,
                //        model.RegionMsk,
                //        model.RegionSpb,
                //        model.RegionRus,
                //        (int)model.DistributionClassifierId
                //    );

                //    if (model.SellingSumPart > 0 && !checkDataForAvgPrice.HasSellingData ||
                //        model.PurchaseSumPart > 0 && !checkDataForAvgPrice.HasPurchaseData)
                //    {
                //        throw new InvalidOperationException("Нет средней цены");
                //    }
                //}

                //Сохраняем
                var userGuid = new Guid(User.Identity.GetUserId());

                model.SetCountRule(rule, userGuid);

                context.SaveChanges();


                CountRuleView ruleView = context.CountRuleView.Single(r => r.Id == rule.Id);

                //Возвращаем успешный результат
                return CountRuleModel.Create(ruleView);
            }
        }

        #endregion

        // ищем идентичные правила
        private static void CheckSimilarRule(CountRuleModel model, RetailContext context)
        {
            List<long> identicalRulesIdList = context.CheckSimilarRule(model.Id, model.Year, model.Month, model.YearEnd, model.MonthEnd, model.ClassifierId, model.DistributionClassifierId, model.RegionCode);
            
            if (identicalRulesIdList.Any())
                throw new InvalidOperationException(string.Format("Идентичное правило существует ({0})", string.Join(", ", identicalRulesIdList)));
        }

        // ищем зависимые правила
        private static void CheckRelatedRule(CountRuleModel model, RetailContext context)
        {
            List<long> chainRulesIdList = context.CheckRelatedRule(model.Id, model.Year, model.Month, model.YearEnd, model.MonthEnd, model.ClassifierId, model.DistributionClassifierId, model.RegionCode);

            if (chainRulesIdList.Any())
                throw new InvalidOperationException(string.Format("Есть зависимые правила ({0})", string.Join(", ", chainRulesIdList)));
        }

        public ActionResult ErrorMessage(string errorMessage)
        {
            return new JsonNetResult(new { isError = true, errorMessage });
        }

    }
}