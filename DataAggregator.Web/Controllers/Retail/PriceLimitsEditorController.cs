using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public sealed class PriceLimitsEditorController : BaseController
    {
        private readonly RetailContext _context;

        public PriceLimitsEditorController()
        {
            _context = new RetailContext();
        }

        ~PriceLimitsEditorController()
        {
            _context.Dispose();
        }

        [HttpGet]
        public ActionResult GetRanges(int month, int year)
        {
            var ranges = _context.PriceCheckView.Where(pcv => 
                                                        pcv.Month == month &&
                                                        pcv.Year == year && 
                                                        pcv.IsActual != false).ToList();

            return new JsonNetResult(ranges);

        }

        public ActionResult EditRange(
            long? priceRangeId,
            int month,
            int year,
            long drugId,
            long ownerTradeMarkId,
            string regionCode,
            decimal? purchasePriceMin,
            decimal? purchasePriceMax,
            decimal? sellingPriceMin,
            decimal? sellingPriceMax
            )
        {
            PriceRange range = GetOrCreatePriceRange(priceRangeId, month, year, drugId, ownerTradeMarkId, regionCode);

            range.PurchasePriceMin = purchasePriceMin;
            range.PurchasePriceMax = purchasePriceMax;
            range.SellingPriceMin = sellingPriceMin;
            range.SellingPriceMax = sellingPriceMax;

            _context.SaveChanges();

            return new JsonNetResult(new {priceRangeId = range.Id});
        }

        private PriceRange GetOrCreatePriceRange(long? priceRangeId, int month, int year, long drugId, long ownerTradeMarkId,
            string regionCode)
        {

            if (priceRangeId.HasValue)
                return _context.PriceRange.First(pr => pr.Id == priceRangeId);

            PriceRange range = _context.PriceRange
                .FirstOrDefault(pr =>
                    pr.RegionCode == regionCode &&
                    pr.Year == year &&
                    pr.Month == month &&
                    pr.DrugId == drugId &&
                    pr.OwnerTradeMarkId == ownerTradeMarkId);

            if (range != null)
                return range;

            range = new PriceRange();
            _context.PriceRange.Add(range);

            range.Month = month;
            range.Year = year;
            range.DrugId = drugId;
            range.OwnerTradeMarkId = ownerTradeMarkId;
            range.RegionCode = regionCode;
            range.IsActual = true;

            return range;
        }
    }
}