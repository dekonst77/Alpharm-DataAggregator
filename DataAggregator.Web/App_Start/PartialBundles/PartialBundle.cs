using System.Web.Optimization;

namespace DataAggregator.Web.PartialBundles
{
    public class PartialBundle : Bundle
    {
        public PartialBundle(string moduleName, string virtualPath)
            : base(virtualPath, new PartialTransform(moduleName))
        {
        }
    }
}