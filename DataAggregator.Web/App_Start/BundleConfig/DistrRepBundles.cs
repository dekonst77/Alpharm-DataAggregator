using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web.App_Start.BundleConfig
{
    public static class DistrRepBundles
    {
        /// <summary>
        /// Зарегистрировать bundles ОФД
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {

            bundles.Add(new ComplexScriptBundle("~/bundles/DistrRep")
                .Include("~/Scripts/DistrRep/DistrRepController.js")
             );


            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/DistrRep/markup")
              .IncludeDirectory("~/Views/DistrRep", "*.html", true)
          );

        }
    }
}