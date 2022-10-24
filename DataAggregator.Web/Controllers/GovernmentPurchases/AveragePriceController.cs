using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.AveragePrice;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc; 

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class AveragePriceController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }
        ~AveragePriceController()
        {
            _context.Dispose();
            Dispose(false);
        }

        [HttpPost]
        public ActionResult GetRegionNames(long? parentId)
        {
            var regionNames = _context.RegionName.Where(rn => rn.ParentId == parentId).ToList();
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = regionNames
            };
        }

        [HttpPost]
        public ActionResult GetAveragePrices(SelectedRegionJson selectedRegion, SelectedDrugJson selectedDrug, SelectedPeriod selectedPeriod)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.AppendLine("where 1 = 1");

            if (selectedRegion.SelectedRegionId != null)
            {
                whereBuilder.AppendLine(string.Format("and ap.RegionId in (select TargetRegionId from RegionHighLevelLink where HighLevelRegionId = {0})", selectedRegion.SelectedRegionId));
            }

            if (selectedDrug.ClassifierId != null)
            {
                whereBuilder.AppendLine(string.Format("and ap.ClassifierId = {0}", selectedDrug.ClassifierId));
            }

            if (selectedPeriod.Year.HasValue)
            {
                whereBuilder.AppendLine(string.Format("and ap.Year = {0}", selectedPeriod.Year));
            }

            if (selectedPeriod.Month.HasValue)
            {
                whereBuilder.AppendLine(string.Format("and ap.Month = {0}", selectedPeriod.Month));
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.GetAveragePriceList(whereBuilder.ToString())
            };
        }

        [HttpPost]
        public ActionResult SaveAveragePrice(long id, string price)
        {
            var c = new CultureInfo("ru");
            c.NumberFormat.NumberDecimalSeparator = ",";
            var clearPrice = price.Trim().Replace(".", ",");
            var ap = _context.PurchaseAveragePrice.Single(a => a.Id == id);
            ap.Price = decimal.Parse(clearPrice, c);
            ap.LastChangedDate = DateTime.Now;
            ap.LastChangedUserId = new Guid(User.Identity.GetUserId());
            _context.SaveChanges();
            return null;
        }
    }
}