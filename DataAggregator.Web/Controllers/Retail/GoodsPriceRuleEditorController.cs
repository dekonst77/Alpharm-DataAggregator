using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Domain.Model.GoodsData.QueryModel;
using DataAggregator.Web.Models.Retail.CommonPriceRuleEditor;
using DataAggregator.Web.Models.Retail.GoodsPriceRuleEditor;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class GoodsPriceRuleEditorController : BaseController
    {
        [HttpGet]
        public ActionResult GetPriceRuleList(int month, int year)
        {
            List<GoodsPriceRuleListView> priceRuleList;
            using (var context = new GoodsDataContext(APP))
                priceRuleList = context.GoodsPriceRuleListView.Where(pr => pr.Year == year && pr.Month == month).ToList();

            return new JsonNetResult(priceRuleList);
        }

        [HttpPost]
        public async Task<ActionResult> Initialize(GoodsPriceRuleModel model)
        {
            if (model == null)
                throw new ApplicationException("Модель пустая");

            string regionName = null;
            var gridData = new List<GoodsPriceRuleGridModel>();

            List<PriceRuleModelDictionary> moscow;
            List<PriceRuleModelDictionary> saintPetersburg;

            using (var goodsContext = new GoodsDataContext(APP))
            {
                //Регион
                if (model.GoodsId != null && model.OwnerTradeMarkId != null && model.PackerId != null)
                {
                    var region = goodsContext.RegionPM12View.Single(r => r.RegionPM12 == model.RegionCode);

                    regionName = GetRegionFullName(region);

                    int year = model.Year;

                    if (year > 2000)
                        year = year - 2000;

                    //Данные
                    List<GoodsCalcRuleModel> goods = goodsContext.SearchGoodsPriceRuleModel(model.GoodsId.Value, model.OwnerTradeMarkId.Value, model.PackerId.Value, year, model.Month);

                    if (goods.Count != 1)
                        return BadRequest("Ошибка правила по уникальности GOP");

                    gridData = goods.Select(r => new GoodsPriceRuleGridModel
                    {
                        GoodsId = r.GoodsId,
                        OwnerTradeMarkId = r.OwnerTradeMarkId,
                        PackerId = r.PackerId,
                        GoodsDescription = r.GoodsDescription,
                        OwnerTradeMark = r.OwnerTradeMark,
                        Packer = r.Packer
                    }).ToList();
                }

                //Москва
                moscow = await GetData(goodsContext, "77.");

                //Санкт-Петербург
                saintPetersburg = await GetData(goodsContext, "78.");
            }

            //Возвращаем результат
            return new JsonNetResult(new { Region = regionName, GridData = gridData, Moscow = moscow, SaintPetersburg = saintPetersburg });
        }

        private async Task<List<PriceRuleModelDictionary>> GetData(GoodsDataContext context, string partOfRegion)
        {
            List<RegionPM12View> regions = await context.SearchRegionAsync(partOfRegion);

            List<PriceRuleModelDictionary> data = regions
                .Select(r => new PriceRuleModelDictionary { RegionCode = r.RegionPM12, Region = GetRegionFullName(r) })
                .ToList();

            return data;
        }

        private static string GetRegionFullName(RegionPM12View r)
        {
            return string.Format("{0} {1}", r.RegionPM12, r.FullName);
        }

        [HttpPost]
        public ActionResult SearchGoods(string value, int year, int month)
        {
            if (year > 2000)
                year = year - 2000;

            List<GoodsCalcRuleModel> drugs;
            using (var context = new GoodsDataContext(APP))
                drugs = context.SearchGoodsPriceRuleModel(value, year, month);

            List<GoodsPriceRuleGridModel> result = drugs
                .Select(r =>
                    new GoodsPriceRuleGridModel
                    {
                        GoodsId = r.GoodsId,
                        OwnerTradeMarkId = r.OwnerTradeMarkId,
                        PackerId = r.PackerId,
                        GoodsDescription = r.GoodsDescription,
                        OwnerTradeMark = r.OwnerTradeMark,
                        Packer = r.Packer
                    }).ToList();

            return new JsonNetResult(result);
        }

        public ActionResult DeletePriceRule(long priceRuleId)
        {
            using (var context = new GoodsDataContext(APP))
            {
                GoodsPriceRule priceRule = context.GoodsPriceRule.Single(pr => pr.Id == priceRuleId);

                context.GoodsPriceRule.Remove(priceRule);

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
        public ActionResult SaveRule(GoodsPriceRuleModel model)
        {
            if (string.IsNullOrEmpty(model.RegionCode) && model.Regions.Count == 0)
                throw new ApplicationException("RegionCode is empty");

            if (model.GoodsId == null || model.OwnerTradeMarkId == null || model.PackerId == null)
                throw new ArgumentException("DOP Id is empty");

            if (model.Regions == null || model.Regions.Count == 0)
                model.Regions = new List<PriceRuleRegionModel>
                {
                    new PriceRuleRegionModel {RegionCode = model.RegionCode}
                };

            if (model.PriceRuleId != null && model.Regions.Count > 1)
                throw new ApplicationException("More than one region");

            using (var context = new GoodsDataContext(APP))
            {
                foreach (var region in model.Regions)
                {


                    GoodsPriceRule priceRule = null;

                    if (model.PriceRuleId != null)
                        priceRule = context.GoodsPriceRule.SingleOrDefault(pr => pr.Id == model.PriceRuleId);

                    var conflictRuleCount = context.GoodsPriceRule.Count(pr =>
                        (model.PriceRuleId == null || pr.Id != model.PriceRuleId) &&
                        pr.Year == model.Year &&
                        pr.Month == model.Month &&
                        pr.RegionCode == region.RegionCode &&
                        pr.GoodsId == model.GoodsId &&
                        pr.OwnerTradeMarkId == model.OwnerTradeMarkId &&
                        pr.PackerId == model.PackerId);

                    if (conflictRuleCount > 0)
                        return BadRequest("Аналогичное правило уже существует!");

                    if (priceRule == null)
                    {
                        priceRule = new GoodsPriceRule
                        {
                            Year = model.Year,
                            Month = model.Month
                        };

                        context.GoodsPriceRule.Add(priceRule);
                    }

                    var userGuid = new Guid(User.Identity.GetUserId());

                    priceRule.RegionCode = region.RegionCode;
                    priceRule.GoodsId = model.GoodsId.Value;
                    priceRule.OwnerTradeMarkId = model.OwnerTradeMarkId.Value;
                    priceRule.PackerId = model.PackerId.Value;
                    priceRule.SellingPriceMin = model.SellingPriceMin;
                    priceRule.SellingPriceMax = model.SellingPriceMax;
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