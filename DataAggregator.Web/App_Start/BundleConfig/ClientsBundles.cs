using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;


namespace DataAggregator.Web.App_Start.BundleConfig
{
    public static class ClientsBundles
    {
        /// <summary>
        /// Зарегистрировать bundles ОФД
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {

            bundles.Add(new ComplexScriptBundle("~/bundles/Clients")

                .Include("~/Scripts/Clients/ClientsController.js")
             );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Clients/markup")
              .IncludeDirectory("~/Views/Clients", "*.html", true)
          );

        }
    }
}