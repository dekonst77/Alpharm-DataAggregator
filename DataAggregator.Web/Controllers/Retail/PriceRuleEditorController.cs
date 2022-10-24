using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.QueryModel;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Web.Models.Retail.CommonPriceRuleEditor;
using DataAggregator.Web.Models.Retail.PriceRuleEditor;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataAggregator.Web.Models.Retail.CountRuleEditor;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class PriceRuleEditorController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult> Initialize(PriceRuleModel model)
        {
            if (model == null)
                throw new ApplicationException("Модель пустая");

            PriceRuleModelDictionary regionModel = null;
            var gridData = new List<PriceRuleGridModel>();

            using (var context = new RetailContext())
            {
                //Регион
                if (model.ClassifierId != null)
                {
                    var regionView = context.RegionPM12View.Single(r => r.RegionPM12 == model.RegionCode);

                    regionModel = await Task.Run(() => RegionController.CreateRegionModel(regionView));

                    int year = model.Year;

                    if (year > 2000)
                        year -= 2000;

                    //Данные
                    List<CalcRuleModel> drugs = await Task.Run(() => context.SearchDrugPriceRuleModel(model.ClassifierId.Value, year, model.Month));

                    if (drugs.Count != 1)
                        return BadRequest("Ошибка правила по уникальности DOP");

                    gridData = await Task.Run(() => drugs.Select(
                       r => new PriceRuleGridModel
                       {
                           ClassifierId = r.ClassifierId,
                           DrugDescription = r.DrugDescription,
                           OwnerTradeMark = r.OwnerTradeMark,
                           Packer = r.Packer
                       }
                       ).ToList());
                }

                ////Москва
                //moscow = await GetData(context, "77.");

                ////Санкт-Петербург
                //saintPetersburg = await GetData(context, "78.");
            }

            //Возвращаем результат
            return new JsonNetResult(new { Region = regionModel, GridData = gridData });//, Moscow = moscow, SaintPetersburg = saintPetersburg });
        }

        //private async Task<List<PriceRuleModelDictionary>> GetData(RetailContext context, string partOfRegion)
        //{
        //    List<RegionPM12View> regions = await context.SearchRegionAsync(partOfRegion);

        //    List<PriceRuleModelDictionary> data = regions
        //        .Select(r => new PriceRuleModelDictionary { RegionCode = r.RegionPM12, Region = GetRegionFullName(r) })
        //        .ToList();

        //    return data;
        //}

        [HttpPost]
        public ActionResult SearchDrug(string value, int year, int month)
        {
            if (year > 2000)
                year -= 2000;

            List<CalcRuleModel> drugs;
            using (var context = new RetailContext(APP))
                drugs = context.SearchDrugPriceRuleModel(value, year, month);

            return CreateDrugList(drugs, true);
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
                    DrugDescription = r.DrugDescription,
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



        private static string GetRegionFullName(RegionPM12View r)
        {
            return string.Format("{0} {1}", r.RegionPM12, r.FullName);
        }

        [HttpGet]
        public ActionResult GetPriceRuleList(int month, int year)
        {
            List<PriceRuleListView> priceRuleList;
            using (var context = new RetailContext(APP))
                priceRuleList = context.PriceRuleListView.Where(pr => pr.Year == year && pr.Month == month).ToList();

            return new JsonNetResult(priceRuleList);
        }

        public ActionResult DeletePriceRule(long priceRuleId)
        {
            using (var context = new RetailContext(APP))
            {
                PriceRule priceRule = context.PriceRule.Single(pr => pr.Id == priceRuleId);

                context.PriceRule.Remove(priceRule);

                var calcInfo = context.CalcInfo.FirstOrDefault(ci => ci.Year == priceRule.Year && ci.Month == priceRule.Month);
                if (calcInfo != null)
                {
                    calcInfo.DateUpdatePriceRules = DateTime.Now;
                }

                context.SaveChanges();
            }

            return new JsonNetResult(null);
        }


        [HttpPost]
        public ActionResult SaveRule(PriceRuleModel model)
        {
            if (string.IsNullOrEmpty(model.RegionCode) && model.Regions.Count == 0)
                throw new ApplicationException("RegionCode is empty");

            if (model.ClassifierId == null)
                throw new ArgumentException("DOP Id is empty");

            if (model.Regions == null || model.Regions.Count == 0)
                model.Regions = new List<PriceRuleRegionModel>
                {
                    new PriceRuleRegionModel {RegionCode = model.RegionCode}
                };

            if (model.PriceRuleId != null && model.Regions.Count > 1)
                throw new ApplicationException("More than one region");

            using (var context = new RetailContext(APP))
            {
                foreach (var region in model.Regions)
                {
                    PriceRule priceRule = null;

                    if (model.PriceRuleId != null)
                        priceRule = context.PriceRule.SingleOrDefault(pr => pr.Id == model.PriceRuleId);

                    var conflictRuleCount = context.PriceRule.Count(pr =>
                        (model.PriceRuleId == null || pr.Id != model.PriceRuleId) &&
                        pr.Year == model.Year &&
                        pr.Month == model.Month &&
                        pr.RegionCode == region.RegionCode &&
                        pr.ClassifierId == model.ClassifierId);

                    if (conflictRuleCount > 0)
                        return BadRequest("Аналогичное правило уже существует!");

                    if (priceRule == null)
                    {
                        priceRule = new PriceRule
                        {
                            Year = model.Year,
                            Month = model.Month
                        };

                        context.PriceRule.Add(priceRule);
                    }

                    var userGuid = new Guid(User.Identity.GetUserId());

                    priceRule.RegionCode = region.RegionCode;
                    priceRule.ClassifierId = model.ClassifierId.Value;
                    priceRule.SellingPriceMin = model.SellingPriceMin;
                    priceRule.SellingPriceMax = model.SellingPriceMax;
                    priceRule.PurchasePriceMin = model.PurchasePriceMin;
                    priceRule.PurchasePriceMax = model.PurchasePriceMax;
                    priceRule.UserId = userGuid;
                    priceRule.Comment = model.Comment;
                    priceRule.Date = DateTime.Now;

                    var calcInfo = context.CalcInfo.FirstOrDefault(ci => ci.Year == priceRule.Year && ci.Month == priceRule.Month);
                    if (calcInfo != null)
                    {
                        calcInfo.DateUpdatePriceRules = priceRule.Date;
                    }
                }

                context.SaveChanges();
            }

            return new EmptyResult();
        }
    }
}