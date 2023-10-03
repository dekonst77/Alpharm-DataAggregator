using DataAggregator.Web.ComplexBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;


namespace DataAggregator.Web.App_Start.BundleConfig
{
    public static class AlphaVisionBundles
    {
        /// <summary>
        /// Зарегистрировать bundles AlphaVision
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            //Controller
            bundles.Add(new ComplexScriptBundle("~/bundles/AlphaVision")
                .IncludeDirectory("~/Scripts/AlphaVision", "*.js", true));

            //Стили          
            bundles.Add(new ComplexStyleBundle("~/Content/AlphaVision/css")
            //   .IncludeDirectory("~/Content/AlphaVision", "*.css", true)
               .IncludeDirectory("~/Content/AlphaVision", "*.scss", true)
               );



            //HTML
            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/AlphaVision/markup")
                .IncludeDirectory("~/Views/AlphaVision", "*.html", true));
        }
            
    }
}


