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
using DataAggregator.Web.Models.Retail.CountRuleFullVolumeEditor;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class CountRuleFullVolumeEditorController : BaseController
    {
        /// <summary>
        /// Загрузка правил
        /// </summary>
        [HttpGet]
        public ActionResult LoadRules(ActualityType actualityType)
        {
            return new JsonNetResult(LoadCountRuleFullVolumeModels(actualityType));
        }

        /// <summary>
        /// Загрузка правил
        /// </summary>
        /// <param name="actualityType">Тип актуальности правил</param>
        /// <returns></returns>
        private static List<CountRuleFullVolumeModel> LoadCountRuleFullVolumeModels(ActualityType actualityType)
        {
            using (var context = new RetailContext())
            {
                List<CountRuleFullVolumeView> data = context.CountRuleFullVolumeView
                    .OrderByDescending(cr => cr.Id)
                    .ToList();

                if (actualityType == ActualityType.Actual)
                    data = data.Where(IsActual).ToList();

                if (actualityType == ActualityType.NotActual)
                    data = data.Where(s => !IsActual(s)).ToList();

                return data.Select(CountRuleFullVolumeModel.Create).ToList();
            }
        }

        private static bool IsActual(CountRuleFullVolumeView s)
        {
            return s.ClassifierId != s.DistributionClassifierId;
        }

        [HttpPost]
        public ActionResult SearchDrugInRus(long brandId)
        {
            using (var context = new RetailContext())
            {
                List<CalcRuleModel> drugs = context.SearchDrugInRusCountRuleFullVolumeModel(brandId);

                return CreateDrugList(drugs, true);
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(int id)
        {
            using (var context = new RetailContext())
            {
                CountRuleFullVolume rule = context.CountRuleFullVolume.Single(cr => cr.Id == id);

                context.CountRuleFullVolume.Remove(rule);

                context.SaveChanges();

                // Возвращаем успешный результат
                return new JsonNetResult(true);
            }
        }

        private static ActionResult CreateDrugList(List<CalcRuleModel> drugs, bool withSum)
        {
            var result = new List<CountRuleDrugData>();

            if (drugs != null && drugs.Count > 0)
            {
                result = drugs.Select(r => new CountRuleDrugData()
                {
                    DrugId = r.DrugId,
                    OwnerTradeMarkId = r.OwnerTradeMarkId,
                    PackerId = r.PackerId,
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
        /// Сохраняем изменения в правиле
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRow(CountRuleFullVolumeModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            try
            {
                CountRuleFullVolumeModel newModel = ChangeCountRuleFullVolumeModel(model);
                
                //Возвращаем успешный результат
                return new JsonNetResult(newModel);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        private CountRuleFullVolumeModel ChangeCountRuleFullVolumeModel(CountRuleFullVolumeModel model)
        {
            using (var context = new RetailContext())
            {
                // ищем идентичные правила
                CheckSimilarRule(model, context);

                model.ChangeDate = DateTime.Now;

                CountRuleFullVolume rule;
                
                if(model.Id.HasValue)
                    rule = context.CountRuleFullVolume.First(cr => cr.Id == model.Id);
                else
                {
                    rule = new CountRuleFullVolume();
                    context.CountRuleFullVolume.Add(rule);
                }

                ModelMapper.Mapper.Map(model, rule);

                rule.ChangeUserId = new Guid(User.Identity.GetUserId());

                context.SaveChanges();

                // Делаем выборку из базы т.к. нам нужно именно представление
                CountRuleFullVolumeView ruleView = context.CountRuleFullVolumeView.Single(r => r.Id == rule.Id);

                CountRuleFullVolumeModel newModel = CountRuleFullVolumeModel.Create(ruleView);

                return newModel;
            }
        }

        private static void CheckSimilarRule(CountRuleFullVolumeModel model, RetailContext context)
        {
            List<long> identicalRulesIdList = context.CountRuleFullVolume
                .Where(cr =>
                    cr.Id != model.Id &&
                    cr.DrugId == model.DrugId &&
                    cr.OwnerTradeMarkId == model.OwnerTradeMarkId &&
                    cr.PackerId == model.PackerId
                )
                .Select(cr => cr.Id)
                .ToList();

            if (identicalRulesIdList.Any())
                throw new InvalidOperationException(string.Format("Идентичное правило существует ({0})",
                    string.Join(", ", identicalRulesIdList)));
        }

        private static ActionResult ErrorMessage(string errorMessage)
        {
            return new JsonNetResult(new { isError = true, errorMessage });
        }
    }
}