using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Domain.Model.GoodsData.QueryModel;
using DataAggregator.Web.Models.Retail;
using DataAggregator.Web.Models.Retail.GoodsCountRuleEditor;
using DataAggregator.Web.Models.Retail.GoodsCountRuleFullVolumeEditor;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class GoodsCountRuleFullVolumeEditorController : BaseController
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
        private List<GoodsCountRuleFullVolumeModel> LoadCountRuleFullVolumeModels(ActualityType actualityType)
        {
            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCountRuleFullVolumeView> data = context.GoodsCountRuleFullVolumeView
                    .OrderByDescending(cr => cr.Id)
                    .ToList();

                if (actualityType == ActualityType.Actual)
                    data = data.Where(IsActual).ToList();

                if (actualityType == ActualityType.NotActual)
                    data = data.Where(s => !IsActual(s)).ToList();

                return data.Select(GoodsCountRuleFullVolumeModel.Create).ToList();
            }
        }

        private static bool IsActual(GoodsCountRuleFullVolumeView s)
        {
            return s.ClassifierId != s.DistributionClassifierId;
        }

        [HttpPost]
        public ActionResult SearchGoodsInRus(long brandId)
        {
            using (var context = new GoodsDataContext(APP))
            {
                List<GoodsCalcRuleModel> drugs = context.SearchGoodsInRusCountRuleFullVolumeModel(brandId);

                return CreateGoodsList(drugs, true);
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(int id)
        {
            using (var context = new GoodsDataContext(APP))
            {
                GoodsCountRuleFullVolume rule = context.GoodsCountRuleFullVolume.Single(cr => cr.Id == id);

                context.GoodsCountRuleFullVolume.Remove(rule);

                context.SaveChanges();

                // Возвращаем успешный результат
                return new JsonNetResult(true);
            }
        }

        private static ActionResult CreateGoodsList(List<GoodsCalcRuleModel> drugs, bool withSum)
        {
            var result = new List<GoodsCountRuleData>();

            if (drugs != null && drugs.Count > 0)
            {
                result = drugs.Select(r => new GoodsCountRuleData()
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

            return new JsonNetResult(result);
        }

        private static decimal? GetDecimalData(decimal? source, bool withSum)
        {
            return withSum && source != null ? Math.Round((decimal)source, 2) : (decimal?)null;
        }



        /// <summary>
        /// Сохраняем изменения в правиле
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult SaveRow(GoodsCountRuleFullVolumeModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            try
            {
                GoodsCountRuleFullVolumeModel newModel = ChangeCountRuleFullVolumeModel(model);
                
                //Возвращаем успешный результат
                return new JsonNetResult(newModel);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMessage(ex.Message);
            }
        }

        private GoodsCountRuleFullVolumeModel ChangeCountRuleFullVolumeModel(GoodsCountRuleFullVolumeModel model)
        {
            using (var context = new GoodsDataContext(APP))
            {
                // ищем идентичные правила
                CheckSimilarRule(model, context);

                model.ChangeDate = DateTime.Now;

                GoodsCountRuleFullVolume rule;
                
                if(model.Id.HasValue)
                    rule = context.GoodsCountRuleFullVolume.First(cr => cr.Id == model.Id);
                else
                {
                    rule = new GoodsCountRuleFullVolume();
                    context.GoodsCountRuleFullVolume.Add(rule);
                }

                ModelMapper.Mapper.Map(model, rule);

                rule.ChangeUserId = new Guid(User.Identity.GetUserId());

                context.SaveChanges();

                // Делаем выборку из базы т.к. нам нужно именно представление
                GoodsCountRuleFullVolumeView ruleView = context.GoodsCountRuleFullVolumeView.Single(r => r.Id == rule.Id);

                GoodsCountRuleFullVolumeModel newModel = GoodsCountRuleFullVolumeModel.Create(ruleView);

                return newModel;
            }
        }

        private static void CheckSimilarRule(GoodsCountRuleFullVolumeModel model, GoodsDataContext context)
        {
            List<long> identicalRulesIdList = context.GoodsCountRuleFullVolume
                .Where(cr =>
                    cr.Id != model.Id &&
                    cr.GoodsId == model.GoodsId &&
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