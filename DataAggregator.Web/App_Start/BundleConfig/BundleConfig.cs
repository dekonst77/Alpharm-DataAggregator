using BundleTransformer.Core.Resolvers;
using DataAggregator.Web.App_Start.BundleConfig;
using System.Web.Optimization;
using DataAggregator.Web.ComplexBundles;

namespace DataAggregator.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleResolver.Current = new CustomBundleResolver();

            AdditionalComponentsBundles.Register(bundles);

            MainBundles.Register(bundles);

            SystematizationBundles.Register(bundles);
            ClassifierBundles.Register(bundles);
            GovernmentPurchasesBundles.Register(bundles);
            RetailBundles.Register(bundles);
            ManagementBundles.Register(bundles);

            OFDBundles.Register(bundles);
            ClientsBundles.Register(bundles);
            DistrRepBundles.Register(bundles);

            //ЛПУ
            LPUBundles.Register(bundles);

            ADD_GS(bundles);

            bundles.Add(new ComplexScriptBundle("~/bundles/Projects")
                .Include("~/Scripts/Management/ProjectController.js"));

            BundleTable.EnableOptimizations = true;
        }

        internal static void ADD_GS(BundleCollection bundles)
        {
            bundles.Add(new ComplexScriptBundle("~/bundles/GS")
.Include("~/Scripts/GS/GSController.js")
             );
        }
    }
}