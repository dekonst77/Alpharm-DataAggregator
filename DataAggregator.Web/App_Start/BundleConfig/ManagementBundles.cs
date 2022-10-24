using DataAggregator.Web.ComplexBundles;
using System.Web.Optimization;

namespace DataAggregator.Web
{
    /// <summary>
    /// Регистратор bundles менеджмента
    /// </summary>
    public static class ManagementBundles
    {
        /// <summary>
        /// Зарегистрировать bundles менеджмента
        /// </summary>
        internal static void Register(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/Management")
                .Include("~/Scripts/Management/DepartmentDictionaryController.js"));

            bundles.Add(new ComplexScriptBundle("~/bundles/Global")
            .Include("~/Scripts/Common/GlobalController.js"));

        }
    }
}