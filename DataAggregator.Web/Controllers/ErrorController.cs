namespace DataAggregator.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult Unauthorized()
        {
            Response.StatusCode = 401;
            return View();
        }
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult Forbidden()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}
