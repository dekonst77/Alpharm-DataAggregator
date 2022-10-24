using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles систематизации
    /// </summary>
    public static class SystematizationBundles
    {
        /// <summary>
        /// Зарегистрировать bundles систематизации
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/Systematization")
                .Include("~/Scripts/Systematization/PrioritetWordsController.js")
                // Обработка данных
                .Include("~/Scripts/Systematization/DrugGoodClassifier.js")
                .Include("~/Scripts/Systematization/HelpController.js")
                .Include("~/Scripts/Systematization/ClassifierFilterController.js")
                .Include("~/Scripts/Systematization/DrugFilterController.js")
                .Include("~/Scripts/Systematization/SystematizationSetPromoController.js")

                // Обработка доп. ассортимента
                .Include("~/Scripts/GoodsSystematization/GoodsSystematizationController.js")
                .Include("~/Scripts/GoodsSystematization/GoodsClassifierFilterController.js")
                .Include("~/Scripts/GoodsSystematization/GoodsFilterController.js")
                .Include("~/Scripts/GoodsSystematization/GoodsCategorySelectorController.js")

                // Настройки периодов
                .Include("~/Scripts/Systematization/PeriodsSettings/PeriodsSettingsController.js")
            );

            bundles.Add(new ComplexStyleBundle("~/Content/Systematization/css")

                // Обработка данных
                .Include("~/Content/Systematization/Systematization.scss")

                // Обработка доп. ассортимента
                .Include("~/Content/GoodsSystematization/GoodsSystematization.scss")

                // Настройки периодов
                .Include("~/Content/Systematization/PeriodsSettings.scss")
                .Include("~/Content/common/resizable.scss")

            );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Systematization/markup")
                .IncludeDirectory("~/Views/Systematization", "*.html", true)
                .IncludeDirectory("~/Views/GoodsSystematization", "*.html", true)
            );
        }
    }
}