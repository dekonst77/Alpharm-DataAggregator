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

        void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception == null)
            {
                return;
            }

            #region source info

            var httpContext = ((MvcApplication)sender).Context;
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = currentRouteData?.Values["controller"].ToString() ?? string.Empty;
            var currentAction = currentRouteData?.Values["action"].ToString() ?? string.Empty;

            #endregion

            #region запись в журнал

            #endregion

            #region choice controller and action

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";

            HttpException httpException = exception as HttpException;

            if (httpException == null) // если ошибка не http
            {
                if (Context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
                    routeData.Values.Add("action", "Unauthorized");
                else
                    routeData.Values.Add("action", "General");
            }
            else
                //It's an Http Exception, Let's handle it.
                switch (httpException.GetHttpCode())
                {
                    case 401:
                        // Unauthorized
                        routeData.Values.Add("action", "Unauthorized");
                        break;
                    case 403:
                        // Forbidden - доступ запрещён
                        routeData.Values.Add("action", "Forbidden");
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
                        routeData.Values.Add("action", "HttpError");
                        break;
                }

            routeData.Values.Add("exception", exception);

            #endregion

            routeData.Values.Add("url", Context.Request.Url.OriginalString);

            #region подготовка ответа

            // Clear the error on server.
            httpContext.ClearError();
            
            httpContext.Response.Clear();

            // to disable IIS custom errors
            httpContext.Response.TrySkipIisCustomErrors = true;

            #endregion

            var errorController = new ErrorController();

            // Pass exception details, current Controller, current Action to the target error View.
            errorController.ViewData.Model = new HandleErrorInfo(exception, currentController, currentAction);

            // Call target Controller and pass the routeData.
            ((IController)errorController).Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
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
