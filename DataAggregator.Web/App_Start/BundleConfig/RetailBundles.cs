using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles розницы
    /// </summary>
    public static class RetailBundles
    {
        /// <summary>
        /// Зарегистрировать bundles розницы
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/Retail")

                // Редактор источников и шаблонов
                .Include("~/Scripts/Retail/RetailTemplatesController.js")

                // Загрузка файлов
                .Include("~/Scripts/Retail/FileInfo/FileInfoController.js")

                // Справочник аптек
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/SourcePharmaciesEditorController.js")
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/SourcePharmaciesEditController.js")
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/DeletePharmaciesController.js")
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/MergePharmaciesController.js")
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/GroupEditController.js")
                .Include("~/Scripts/Retail/SourcePharmaciesEditor/DeleteGroupController.js")

                // Черный список аптека-бренд
                .Include("~/Scripts/Retail/PharmacyBrandBlackList/PharmacyBrandBlackListController.js")
                .Include("~/Scripts/Retail/PharmacyBrandBlackList/DeletePositionsController.js")
                //Черный список аптек
                .Include("~/Scripts/Retail/PharmacyWithoutAverage/PharmacyWithoutAverageController.js")
                //Черный список брендов
                .Include("~/Scripts/Retail/SourceBrandBlackList/SourceBrandBlackListController.js")

                // Редактор цен
                .Include("~/Scripts/Retail/PriceLimitsEditorController.js")

                // Редактор количеств
                .Include("~/Scripts/Retail/CountCheckController.js")

                // Корректировка цен на итоговых данных
                .Include("~/Scripts/Retail/PriceRuleEditor/PriceRuleEditorController.js")
                .Include("~/Scripts/Retail/PriceRuleEditor/PriceRuleViewController.js")

                // Корректировка количеств на итоговых данных
                .Include("~/Scripts/Retail/CountRuleEditor/CountRuleEditorController.js")
                .Include("~/Scripts/Retail/CountRuleEditor/SearchDrugController.js")
                .Include("~/Scripts/Retail/CountRuleEditor/TransferRuleController.js")

                // Корректировка количеств на итоговых данных - 100%
                .Include("~/Scripts/Retail/CountRuleFullVolumeEditor/CountRuleFullVolumeEditorController.js")
                .Include("~/Scripts/Retail/CountRuleFullVolumeEditor/SearchDrugFullVolumeController.js")

                // Корректировка наценок по умолчанию
                .Include("~/Scripts/Retail/MarkupDefaultEditorController.js")

                // Корректировка цен доп. ассортимента на итоговых данных
                .Include("~/Scripts/Retail/GoodsPriceRuleEditor/GoodsPriceRuleEditorController.js")
                .Include("~/Scripts/Retail/GoodsPriceRuleEditor/GoodsPriceRuleViewController.js")

                // Корректировка количеств доп. ассортимента на итоговых данных
                .Include("~/Scripts/Retail/GoodsCountRuleEditor/GoodsCountRuleEditorController.js")
                .Include("~/Scripts/Retail/GoodsCountRuleEditor/SearchGoodsController.js")
                .Include("~/Scripts/Retail/GoodsCountRuleEditor/GoodsTransferRuleController.js")

                // Корректировка количеств доп. ассортимента на итоговых данных - 100%
                .Include("~/Scripts/Retail/GoodsCountRuleFullVolumeEditor/GoodsCountRuleFullVolumeEditorController.js")
                .Include("~/Scripts/Retail/GoodsCountRuleFullVolumeEditor/SearchGoodsFullVolumeController.js")

                // Поиск в исходных данных по тексту
                .Include("~/Scripts/Retail/SearchRawDataByDrugClearController.js")

                // Поиск в исходных данных по классификатору
                .Include("~/Scripts/Retail/SearchRawDataByClassifierController.js")

                // Поиск в исходных данных по классификатору доп. ассортимента
                .Include("~/Scripts/Retail/SearchRawDataByGoodsClassifierController.js")


                // Форма отчетности ретейла
                .Include("~/Scripts/Retail/RetailReport/RetailReportController.js")

                //Выпуск ретейл
                .Include("~/Scripts/Retail/RetailCalculation/RetailCalculationController.js")

                .Include("~/Scripts/Retail/EcomController.js")

                //CTM
                .Include("~/Scripts/Retail/CTM/CTMController.js")

                // Продажи SKU по СФ
                .Include("~/Scripts/Retail/SalesSKUbySF/SalesSKUBySFController.js")

                //Фиксация правил
                .Include("~/Scripts/Retail/RulesCommit/RulesCommitController.js")
                .Include("~/Scripts/Classifier/CheckedController.js")
            );

            bundles.Add(new ComplexStyleBundle("~/Content/Retail/css")

                // Редактор источников и шаблонов
                .Include("~/Content/Retail/RetailTemplates.scss")

                // Черный список аптека-бренд
                .Include("~/Content/Retail/PharmacyBrandBlackList.css")

                // Корректировка количеств на итоговых данных & Корректировка количеств на итоговых данных - 100%
                .Include("~/Content/Retail/CountRuleEditor.scss")

                // Корректировка наценок по умолчанию
                .Include("~/Content/Retail/MarkupDefaultEditor.css")

                // Отчеты
                .Include("~/Content/Retail/RetailReport.css")
            );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Retail/markup")
                .IncludeDirectory("~/Views/Retail", "*.html", true)
            );
        }
    }
}