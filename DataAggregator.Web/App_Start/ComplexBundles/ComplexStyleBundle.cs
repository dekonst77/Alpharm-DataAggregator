using BundleTransformer.Core;
using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using System.Web.Optimization;

namespace DataAggregator.Web.ComplexBundles
{
    /// <summary>
    /// Аналог CustomStyleBundle, но добавлен Orderer = new NullOrderer();
    /// </summary>
    public sealed class ComplexStyleBundle : Bundle
    {
        public ComplexStyleBundle(string virtualPath)
            : this(virtualPath, null)
        {
        }

        public ComplexStyleBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath, BundleTransformerContext.Current.Styles.GetDefaultTransformInstance())
        {
            Builder = new NullBuilder();
            Orderer = new NullOrderer();
        }
    }
}