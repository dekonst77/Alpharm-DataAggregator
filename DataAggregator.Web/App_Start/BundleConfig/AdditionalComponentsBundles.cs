using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles дополнительных компонентов
    /// </summary>
    public static class AdditionalComponentsBundles
    {

        private const string Language = "ru-RU";

        /// <summary>
        /// Зарегистрировать bundles дополнительных компонентов
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            #region Common

            bundles.Add(new ComplexScriptBundle("~/bundles/common/ru-RU")
                .Include("~/Scripts/jquery-3.6.0.js")

                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-aria.js")
                .Include("~/Scripts/angular-route.js")
                .Include("~/Scripts/angular-animate.min.js")
                .Include("~/Scripts/angular-touch.js")
                .Include("~/Scripts/angular-sanitize.js")
                .Include("~/Scripts/angular-resource.js")
                .Include("~/Scripts/angular-messages.js")
                .Include("~/Scripts/i18n/angular-locale_ru-ru.js")

                .Include("~/Scripts/lib/jszip.min.js")

                .Include("~/Scripts/lib/angular-ui/0.4.0/angular-ui.min.js")

                .Include("~/Scripts/lib/bootstrap/3.3.7/js/bootstrap.min.js")
                .Include("~/Scripts/lib/angular-ui-bootstrap/2.5.0/ui-bootstrap-tpls.min.js")
                .Include("~/Scripts/lib/angular-ui-bootstrap/ui-bootstrap.ru-RU.js")

                .Include("~/Scripts/angular-translate.js")
                .Include("~/Scripts/lib/angular-busy/4.1.4/angular-busy.js")
                .Include("~/Scripts/lib/angular-busy/angular-busy.ru-RU.js")
                .Include("~/Scripts/select.min.js")
                .Include("~/Scripts/ui-select-pagination-groups.js")


                .Include("~/Scripts/ng-file-upload-shim.min.js")
                .Include("~/Scripts/ng-file-upload.min.js")
                .Include("~/Scripts/loading-bar.min.js")
                .Include("~/Scripts/lib/ngRightClick.js")                
                .Include("~/Scripts/lib/FileSaver/FileSaver.min.js")
                .Include("~/Scripts/lib/dynamic-number/dynamic-number.js")
                .Include("~/Scripts/lib/datetime.js")
                .Include("~/Scripts/lib/Multiselect/multiselect.js")
      

                .Include("~/Scripts/common.js")
                .Include("~/Scripts/Common/DataAggregator/DataAggregatorModule.js")
                .Include("~/Scripts/Common/DataAggregator/routing.js")
                .Include("~/Scripts/Common/DataAggregator/DataAggregatorController.js")
                .Include("~/Scripts/Account/LoginController.js")
                .Include("~/Scripts/Localization/ru_RU.js")
                .IncludeDirectory("~/Scripts/Common/Constants", "*.js")
                .IncludeDirectory("~/Scripts/Common/Directives", "*.js")
                .IncludeDirectory("~/Scripts/Common/Services", "*.js")
                .IncludeDirectory("~/Scripts/Common/Controllers", "*.js")
                .IncludeDirectory("~/Scripts/Common/Configurators", "*.js")
            );

            bundles.Add(new StyleBundle("~/Content/common/css")
                .Include("~/Scripts/lib/bootstrap/3.3.7/css/bootstrap.min.css")
                .Include("~/Scripts/lib/angular-busy/4.1.4/angular-busy.css")
                .Include("~/Content/loading-bar.min.css")
                .Include("~/Content/common/common.css")
                .Include("~/Content/select.min.css")
            );

            #endregion

            #region ui-grid

            bundles.Add(new ScriptBundle("~/bundles/ui-grid")
                .IncludeDirectory("~/Scripts/lib/ui-grid/4.9.2", "*.js")

                .Include("~/Scripts/lib/ui-grid/ui-grid.ru-RU.js")
                .Include("~/Scripts/lib/ui-grid/ui-grid.module.js")
                
                .IncludeDirectory("~/Scripts/lib/ui-grid/Custom", "*.js")
                .Include("~/Scripts/lib/ui-grid/grunt-scripts/csv.js")
                .Include("~/Scripts/lib/ui-grid/grunt-scripts/vfs_fonts.js")
                .Include("~/Scripts/lib/ui-grid/grunt-scripts/lodash.min.js")
                .Include("~/Scripts/lib/ui-grid/grunt-scripts/excel-builder.dist.js")
            );


            bundles.Add(new StyleBundle("~/Content/ui-grid/css")
                .Include("~/Scripts/lib/ui-grid/4.9.2/ui-grid.css")
                .IncludeDirectory("~/Scripts/lib/ui-grid/4.9.2/css", "*.css")
                .IncludeDirectory("~/Scripts/lib/ui-grid/Custom", "*.css")
            );

            #endregion         

            #region ui-tree

            bundles.Add(new ScriptBundle("~/bundles/ui-tree")
                .Include("~/Scripts/lib/angular-ui-tree/angular-ui-tree.min.js")
            );

            bundles.Add(new StyleBundle("~/Content/ui-tree/css")
                .Include("~/Scripts/lib/angular-ui-tree/angular-ui-tree.min.css")
            );

            #endregion

            #region HotKeys

            bundles.Add(new ScriptBundle("~/bundles/hotkeys")
                .Include("~/Scripts/hotkeys.min.js")
            );

            bundles.Add(new StyleBundle("~/Content/hotkeys/css")
                .Include("~/Content/hotkeys.min.css")
            );

            #endregion

            #region ContextMenu

            bundles.Add(new ScriptBundle("~/bundles/context-menu")
                .Include("~/Scripts/lib/ContextMenu/ng-contextmenu.js")
            );

            #endregion

            #region angularjs-dropdown-multiselect
            bundles.Add(new ScriptBundle("~/bundles/angularjs-dropdown-multiselect")
                .IncludeDirectory("~/Scripts/lib/angularjs-dropdown-multiselect", "*.js")
            );
            #endregion

            bundles.Add(new PartialBundles.PartialBundle("DataAggregatorModule", "~/Views/Common/markup")
                .Include("~/Views/Shared/_Navigation.html")
                .IncludeDirectory("~/Views/Static","*.html")
            );
        }
    }
}