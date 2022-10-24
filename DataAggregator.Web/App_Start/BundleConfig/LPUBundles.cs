using DataAggregator.Web.ComplexBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DataAggregator.Web.App_Start.BundleConfig
{
    public class LPUBundles
    {
        /// <summary>
        /// Зарегистрировать bundles ОФД
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            //Controller
            bundles.Add(new ComplexScriptBundle("~/bundles/LPU")
                .Include("~/Scripts/LPU/LPUController.js")
                .Include("~/Scripts/LPU/LPUPointController.js")
                .Include("~/Scripts/LPU/LPULicensesController.js"));

            //Стили
            bundles.Add(new ComplexStyleBundle("~/Content/LPU/css")
                // Редактор классификатора
                .Include("~/Content/LPU/LPU.css"));
            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/View/LPU/markup")
            //HTML
              .IncludeDirectory("~/Views/LPU", "*.html", true));
        }
            
    }
}