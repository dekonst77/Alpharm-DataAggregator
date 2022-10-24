using BundleTransformer.Core;
using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using System.Web.Optimization;

namespace DataAggregator.Web.ComplexBundles
{
    /// <summary>
    /// Аналог CustomScriptBundle, но добавлен Orderer = new NullOrderer();
    /// </summary>
    public sealed class ComplexScriptBundle : Bundle
    {
        public ComplexScriptBundle(string virtualPath)
            : this(virtualPath, null)
        {
        }

        public ComplexScriptBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath, BundleTransformerContext.Current.Scripts.GetDefaultTransformInstance())
        {
            Builder = new NullBuilder();
            Orderer = new NullOrderer();
        }
    }
}