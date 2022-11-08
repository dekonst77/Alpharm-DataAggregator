using DataAggregator.Web.Controllers;
using System;
using System.Web;
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
            HttpException httpException = Server.GetLastError() as HttpException;

            if (httpException == null)
            {
                return;
            }

            /*
if (Context.AllErrors != null)
{
    Context.Response.ClearHeaders();
    Context.Response.ClearContent();
    Context.ClearError();
}
*/


            //It's an Http Exception, Let's handle it.

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["StatusCode"] = Context.Response.StatusCode;
            routeData.Values["StatusDescription"] = Context.Response.StatusDescription;

            switch (httpException.GetHttpCode())
            {
                case 401:
                    // Unauthorized
                    routeData.Values.Add("action", "Unauthorized");
                    break;
                case 404:
                    // Page not found.
                    routeData.Values.Add("action", "NotFound");
                    break;
                case 500:
                    // Server error.
                    routeData.Values.Add("action", "HttpError500");
                    break;

                // Here you can handle Views to other error codes.
                // I choose a General error template  
                default:
                    routeData.Values.Add("action", "General");
                    break;
            }

            // Clear the error on server.
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        protected void Application_EndRequest()
        {
            // Unauthorized
            if (Context.Response.StatusCode == 401)
            {
                var routeData = new RouteData();
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = "Unauthorized";

                routeData.Values["StatusCode"] = Context.Response.StatusCode;
                routeData.Values["StatusDescription"] = Context.Response.StatusDescription;

                // Clear the error on server.
                Server.ClearError();

                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;

                // Call target Controller and pass the routeData.
                IController errorController = new ErrorController();
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

                return;
            }
        }

    }
}
