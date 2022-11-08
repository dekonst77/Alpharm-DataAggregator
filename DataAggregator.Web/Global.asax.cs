using DataAggregator.Web.Controllers;
using System;
using System.Net;
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
            Exception exception = Server.GetLastError();

            if (exception == null)
            {
                return;
            }

            HttpException httpException = exception as HttpException;

            Response.Clear();

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";

            if (httpException == null)
            {
                // Unauthorized
                if (Context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    routeData.Values.Add("action", "Unauthorized");
                else
                    routeData.Values.Add("action", "InternalServerError");
            }
            else
                //It's an Http Exception, Let's handle it.
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
                        routeData.Values.Add("action", "InternalServerError");
                        break;

                    // Here you can handle Views to other error codes.
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);

            routeData.Values.Add("url", Context.Request.Url.OriginalString);

            // Clear the error on server.
            Server.ClearError();

            // to disable IIS custom errors
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        protected void Application_EndRequest()
        {
            // Unauthorized
            if (Context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                throw new HttpException((int)HttpStatusCode.Unauthorized, "You are not authorised");
            }
        }

    }
}
