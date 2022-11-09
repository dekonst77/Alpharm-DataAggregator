namespace DataAggregator.Web.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// This controller exists to provide the error page
    /// </summary>
    public class ErrorController : Controller
    {
        private int GetStatusCode(Exception exception)
        {
            HttpException httpException = exception as HttpException;
            return httpException != null ? httpException.GetHttpCode() : (int)HttpStatusCode.InternalServerError;
        }

        public ViewResult Unauthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401

            return View("Unauthorized");
        }
        public ViewResult Forbidden()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden; // 403

            return View();
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound; // 404

            return View("NotFound");
        }
        public ViewResult InternalServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500

            return View("InternalServerError");
        }

        /// <summary>
        /// Обработка ошибок http
        /// </summary>
        /// <returns></returns>
        public ViewResult HttpError()
        {
            Exception exception = (Exception)RouteData.Values["error"];

            Response.StatusCode = GetStatusCode(exception);

            ViewBag.StatusCode = Response.StatusCode;

            // представление всех остальных кодов HTTP
            return View("HttpError");
        }

        /// <summary>
        /// Обработка ошибок сервера
        /// </summary>
        /// <returns></returns>
        public ViewResult General()
        {
            Exception exception = (Exception)RouteData.Values["error"];

            Response.StatusCode = GetStatusCode(exception);

            ViewBag.StatusCode = Response.StatusCode;

            // представление по умолчанию
            return View("General");
        }

    }
}
