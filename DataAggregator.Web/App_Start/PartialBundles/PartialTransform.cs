using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Optimization;

namespace DataAggregator.Web.PartialBundles
{
    public class PartialTransform : IBundleTransform
    {
        private readonly string _moduleName;
        public PartialTransform(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            Bundle bundle = context.BundleCollection.FirstOrDefault(b => b.Path == context.BundleVirtualPath);
            if (bundle == null)
                return;

            var strBundleResponse = new StringBuilder();
            // Javascript module for Angular that uses templateCache 
            strBundleResponse.AppendFormat(@"angular.module('{0}').run(['$templateCache',function(t){{",_moduleName);

            IEnumerable<VirtualFile> virtualFiles = response.Files.Select(s => s.VirtualFile).Where(s => !s.IsDirectory);

            foreach (VirtualFile virtualFile in virtualFiles)
                strBundleResponse.AppendFormat("t.put('{0}','{1}');\r\n", GetTemplateName(virtualFile), GetContext(virtualFile));

            strBundleResponse.Append(@"}]);");

            response.Files = new BundleFile[0];
            response.Content = strBundleResponse.ToString();
            response.ContentType = "text/javascript";
        }

        private static string GetTemplateName(VirtualFile virtualFile)
        {
            string templateName = virtualFile.VirtualPath;

            if (templateName.StartsWith("/"))
                templateName = templateName.Substring(1);

            return templateName;
        }

        private static string GetContext(VirtualFile virtualFile)
        {
            using (Stream stream = virtualFile.Open())
            using (var sr = new StreamReader(stream))
            {
                // Get the partial page, remove line feeds and escape quotes
                string content = sr.ReadToEnd();

                content = content.Replace("\r\n", "").Replace("\n", "").Replace("'", "\\'");

                return content;
            }
        }
    }
}