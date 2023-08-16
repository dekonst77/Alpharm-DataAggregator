using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles классификации
    /// </summary>
    public static class ClassifierBundles
    {
        /// <summary>
        /// Зарегистрировать bundles классификации
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/Classifier")

                // Редактор категорий дополнительного ассортимента
                .Include("~/Scripts/Classifier/GoodsCategoryEditor/GoodsCategoryEditorController.js")
                .Include("~/Scripts/Classifier/GoodsCategoryEditor/GoodsSectionChangeController.js")

                // Редактор свойств дополнительного ассортимента
                .Include("~/Scripts/Classifier/GoodsParametersEditor/GoodsParametersEditorController.js")
                .Include("~/Scripts/Classifier/GoodsParametersEditor/AddParameterViewController.js")

                // Отчёт по классификатору доп. ассортимента
                .Include("~/Scripts/Classifier/GoodsClassifierReport/GoodsClassifierReport.js")

                // Редактор классификатора
                .Include("~/Scripts/Classifier/ClassifierEditor/ClassifierEditorController.js")
                .Include("~/Scripts/Classifier/ClassifierEditor/ClassifierEditorHistoryController.js")
                .Include("~/Scripts/Classifier/Manufacturer/ManufacturerController.js")
                .Include("~/Scripts/Classifier/DDDController.js")
                .Include("~/Scripts/Classifier/DataChangeExcelController.js")
                .Include("~/Scripts/Classifier/ClassifierEditor/ClassifierEditorFilterController.js")
                .Include("~/Scripts/Classifier/ClassifierEditor/AddClassifierInfoController.js")
                .Include("~/Scripts/Classifier/ClassifierEditor/SearchRegistrationCertificateController.js")
                .Include("~/Scripts/Classifier/ClassifierEditor/ChangeInfoController.js")
                .Include("~/Scripts/Classifier/SPRController.js")
                .Include("~/Scripts/Classifier/CheckedController.js")
                .Include("~/Scripts/Classifier/CertificateController.js")
                .Include("~/Scripts/Classifier/GTINController.js")
                .Include("~/Scripts/Classifier/RaspredelenieController.js")
                .Include("~/Scripts/Classifier/SQA/SQAController.js")
                // .Include("~/Scripts/Classifier/ClassifierEditor/AskChangeController.js")

                // Редактор доп. ассортимента
                .Include("~/Scripts/Classifier/GoodsClassifierEditor/GoodsClassifierEditorController.js")
                .Include("~/Scripts/Classifier/GoodsClassifierEditor/GoodsClassifierEditorFilterController.js")
                .Include("~/Scripts/Classifier/GoodsClassifierEditor/AddGoodsClassifierInfoController.js")
                .Include("~/Scripts/Classifier/GoodsClassifierEditor/GoodsChangeInfoController.js")

                // Редактор характеристик
                .Include("~/Scripts/Classifier/ClassifierParametersEditor/ClassifierParametersEditorController.js")
                .Include("~/Scripts/Classifier/ClassifierParametersEditor/ClassifierParametersEditorFilterController.js")
                .Include("~/Scripts/Classifier/ClassifierParametersEditor/ClassifierParametersEditorCellEditController.js")

                // Редактор ФТГ
                .Include("~/Scripts/Classifier/FtgController.js")

                // Редактор ATCWho
                .Include("~/Scripts/Classifier/ATCWhoController.js")

                // Редактор ATCEphmra
                .Include("~/Scripts/Classifier/ATCEphmraController.js")

                // Редактор ATC Бад
                .Include("~/Scripts/Classifier/ATCBaaController.js")

                // Редактор NFC
                .Include("~/Scripts/Classifier/NFCController.js")

                // Переброс данных
                .Include("~/Scripts/Classifier/DataTransfer/DataTransferController.js")
                .Include("~/Scripts/Classifier/DataTransfer/LeftClassifierFilterController.js")
                .Include("~/Scripts/Classifier/DataTransfer/RightClassifierFilterController.js")
                .Include("~/Scripts/Classifier/DataTransfer/DeleteTransferController.js")

                // ЖНВЛП
                .Include("~/Scripts/Classifier/VED/VEDController.js")
                .Include("~/Scripts/Classifier/VED/VEDPeriodController.js")
                .Include("~/Scripts/Classifier/VED/VEDPeriodCopyController.js")

                // Федеральная льгота
                .Include("~/Scripts/Classifier/FederalBenefit/FederalBenefitController.js")
                .Include("~/Scripts/Classifier/FederalBenefit/FederalBenefitPeriodController.js")
                .Include("~/Scripts/Classifier/FederalBenefit/FederalBenefitPeriodCopyController.js")

                // Бионика Медиа
                .Include("~/Scripts/Classifier/Reports/BionicaMediaReportController.js")
                // Job
                .Include("~/Scripts/Classifier/ClassifierRelease/ClassifierReleaseController.js")
                .Include("~/Scripts/Classifier/ClassifierRelease/ClassifierJobInfoController.js")
                // Generic
                .Include("~/Scripts/Classifier/Generic/ClassificationGenericController.js")
                // Маска
                .Include("~/Scripts/Classifier/MaskController.js")

                // Блок «блистеровка» 
                .Include("~/Scripts/Classifier/BlisterBlock/BlisterBlockController.js")

                // Отчет проверки классификатора
                .Include("~/Scripts/Classifier/Reports/CheckClassifireReportController.js")
                .Include("~/Scripts/Classifier/Reports/CheckReportDialogController.js")

                 // Модуль для добавления ДОП ассортимента в БД мониторинг, разработка #11668
                 .Include("~/Scripts/Classifier/AddingDOPMonitoringDatabase/AddingDOPMonitoringDatabase.js")
                 .Include("~/Scripts/Classifier/AddingDOPMonitoringDatabase/DialogSetPlugOffByCategoryController.js")
                 .Include("~/Scripts/Classifier/AddingDOPMonitoringDatabase/DialogSetPlugOnByCategoryController.js")

                 // Модуль для простановки RX, OTC
                 .Include("~/Scripts/Classifier/ClassifierRxOtс/ClassifierRxOtcController.js")

                 // история изменеий в модуле для простановки RX, OTC
                 .Include("~/Scripts/Classifier/ClassifierRxOtс/ClassifierRxOtcHistoryController.js")
                );

            bundles.Add(new ComplexStyleBundle("~/Content/Classifier/css")

                // Редактор классификатора
                .Include("~/Content/Classifier/ClassifierEditor.scss")
                // Редактор доп. ассортимента
                .Include("~/Content/Classifier/GoodsClassifierEditor.scss")
                // SQA
                .Include("~/Content/Classifier/SQA.scss")
            );

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Classifier/markup")
                .IncludeDirectory("~/Views/Classifier", "*.html", true)
            );

        }
    }
}