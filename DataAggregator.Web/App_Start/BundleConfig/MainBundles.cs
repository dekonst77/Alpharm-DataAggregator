using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles главной страницы
    /// </summary>
    public static class MainBundles
    {
        /// <summary>
        /// Зарегистрировать bundles главной страницы
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/Main")
                .Include("~/Scripts/MainController.js")

            );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Main/markup")
                .IncludeDirectory("~/Views/Main", "*.html", true)
            );
        }
    }
}