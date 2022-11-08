namespace DataAggregator.Web.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult Unauthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            ViewBag.StatusCode = RouteData.Values["StatusCode"];
            ViewBag.Path = RouteData.Values["Path"];
            ViewBag.Description = RouteData.Values["StatusDescription"];

            return View();
        }
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            ViewBag.StatusCode = RouteData.Values["StatusCode"];
            ViewBag.Path = RouteData.Values["Path"];
            ViewBag.Description = RouteData.Values["StatusDescription"];

            return View();
        }

        public ActionResult Forbidden()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View();
        }
    }
}
