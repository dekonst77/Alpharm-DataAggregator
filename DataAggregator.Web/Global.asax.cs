using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DataAggregator.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JsonValueProviderFactory jsonValueProviderFactory = null;

            foreach (var factory in ValueProviderFactories.Factories)
            {
                if (factory is JsonValueProviderFactory)
                {
                    jsonValueProviderFactory = factory as JsonValueProviderFactory;
                }
            }

            //remove the default JsonVAlueProviderFactory
            if (jsonValueProviderFactory != null) ValueProviderFactories.Factories.Remove(jsonValueProviderFactory);

            //add the custom one
            ValueProviderFactories.Factories.Add(new CustomJsonValueProviderFactory());

            // Инициализируем mapper
            ModelMapper.Init();
        }

        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            //this.Context.Request.

            //// Transfer the user to the appropriate custom error page
            //HttpException lastErrorWrapper = Server.GetLastError() as HttpException;

            //if (lastErrorWrapper == null)
            //    return;

            //if (lastErrorWrapper.GetHttpCode() == 404)
            //{
            //    Server.Transfer("~/ErrorPages/404.aspx");
            //}
            //else
            //{
            //    Server.Transfer("~/ErrorPages/Oops.aspx");
            //}
        }

        void Application_Error(object sender, EventArgs e)
        {

            // Code that runs when an unhandled error occurs
            //Exception ex = Server.GetLastError();

            //if (ex is HttpUnhandledException)
            //{
            //    // Pass the error on to the error page.
            //    Server.Transfer("ErrorPage.aspx?handler=Application_Error%20-%20Global.asax", true);
            //}
        }
    }
}
