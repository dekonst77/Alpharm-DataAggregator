using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web.App_Start.BundleConfig
{
    public static class OFDBundles
    {
        /// <summary>
        /// Зарегистрировать bundles ОФД
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/OFD")

                // Редактор эталонных цен
                .Include("~/Scripts/OFD/PriceEtalon/PriceEtalonController.js")
                .Include("~/Scripts/OFD/PriceCurrent/PriceCurrentController.js")
                 .Include("~/Scripts/OFD/PriceCurrent/PriceCurrentController_v2.js")
                .Include("~/Scripts/OFD/PriceEtalon/PriceEtalonSearchDrugController.js")
                .Include("~/Scripts/OFD/OFDController.js")
             );

            bundles.Add(new ComplexStyleBundle("~/Content/OFD/css")
                    // Редактор эталонных цен                 
                    .Include("~/Content/OFD/PriceCurrent.scss")
                 );
        
            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/OFD/markup")
              .IncludeDirectory("~/Views/OFD", "*.html", true)
          );

        }
    }
}