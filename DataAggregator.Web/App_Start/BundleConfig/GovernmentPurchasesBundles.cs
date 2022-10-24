using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles госзакупок
    /// </summary>
    public static class GovernmentPurchasesBundles
    {
        /// <summary>
        /// Зарегистрировать bundles госзакупок
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/GovernmentPurchases")
                .IncludeDirectory("~/Scripts/GovernmentPurchases","*.js",true)
            );

            bundles.Add(new ComplexStyleBundle("~/Content/GovernmentPurchases/css")
                .IncludeDirectory("~/Content/GovernmentPurchases", "*.css", true)
                .IncludeDirectory("~/Content/GovernmentPurchases", "*.scss", true)
                );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/GovernmentPurchases/markup")
                .IncludeDirectory("~/Views/GovernmentPurchases", "*.html", true)
            );
        }
    }
}