using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Utils;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace DataAggregator.Web.Controllers
{
    public class BaseController : Controller
    {
        public string APP { get; set; }
        public string APP_Robot = "WWW_UN_Robot";

        public alUser aspUser = null;
        public BaseController()
        {
            APP = "";

            if (User != null && User.Identity != null)
            {
                APP = User.Identity.Name;
                if (User.Identity.IsAuthenticated)
                {
                    aspUser = new alUser();
                    aspUser.SetUser_guid(User.Identity.GetUserId());
                }
            }
        }
        protected override void Initialize(RequestContext requestContext)
        {
            APP = "WWW";
            base.Initialize(requestContext);
            //requestContext.HttpContext.User.Identity.
            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string userName = requestContext.HttpContext.User.Identity.Name;
                APP += "_UN_" + userName;
                aspUser = new alUser();
                aspUser.SetUser_guid(requestContext.HttpContext.User.Identity.GetUserId());
            }
        }
        internal class Result
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public object Data { get; set; }
        }

        internal void LogError(Exception e)
        {
            Log(e);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
                Log(filterContext.Exception, filterContext.Controller.ToString());

            base.OnActionExecuted(filterContext);
        }

        private void Log(Exception exception, string controllerName = null)
        {
            string message = string.Empty;

            Exception exc = exception;

            while (exc != null)
            {
                message += exc.Message;
                exc = exc.InnerException;
            }

            var logEntry = new Logger
            {
                UserId = User.Identity.GetUserId() != null ? new Guid(User.Identity.GetUserId()) : Guid.Empty,
                Project = exception.Source,
                Controller = controllerName ?? ToString(),
                Message = message,
                StackTrace = exception.StackTrace
            };

            logEntry.Save();

        }

        /// <summary>Creates a <see cref="T:System.Web.Mvc.JsonResult" /> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the JSON request behavior.</summary>
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <param name="contentType">The content type (MIME type).</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="behavior">The JSON request behavior</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            JsonResult jsonResult = base.Json(data, contentType, contentEncoding, behavior);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }



        protected static ActionResult ReturnData(object data)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    Success = true,
                    Data = data,
                    ErrorMessage = String.Empty
                }
            };
        }

        protected JsonResult BadRequest(Exception ex)
        {
            string message = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                message += ex.Message;
            }
            return BadRequest(message);
        }
        protected JsonResult BadRequest(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { message });
        }

        protected JsonResult Ok(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { message });
        }
        protected ActionResult Forbidden()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return new EmptyResult();
        }

        protected ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new EmptyResult();
        }

        protected ActionResult NotContent()
        {
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            return new EmptyResult();
        }
        public class JsonResultData
        {
            public ViewDataDictionary Data { get; set; }
            public int count { get; set; }
            public string status { get; set; }
            public bool Success { get; set; }
        }

        static public List<long?> GetListLong(string val)
        {
            List<long?> ret = new List<long?>();
            val = val.Replace(",", " ");
            foreach (var item in val.Split(' '))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ret.Add(Convert.ToInt64(item));
                }
            }
            return ret;
        }
        static public List<string> GetListString(string val)
        {
            List<string> ret = new List<string>();
            val = val.Replace(",", " ");
            foreach (var item in val.Split(' '))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ret.Add("'" + item + "'");
                }
            }
            return ret;
        }
    }

    public class alUser
    {
        Guid _User_guid = Guid.Empty;
        Int16 _UserId = 0;
        string _User_FullName = "";
        string _User_Email = "";
        public alUser()
        { }
        public void SetUser_guid(string ID)
        {
            _User_guid = Guid.Parse(ID);
        }
        public Guid User_guid
        {
            get { return _User_guid; }
        }
        public Int16 UserId
        {
            get { if (_UserId == 0) Load(); return _UserId; }
            set { _UserId = value; }
        }
        public string User_FullName
        {
            get { if (_UserId == 0) Load(); return _User_FullName; }
            set { _User_FullName = value; }
        }
        public string User_Email
        {
            get { if (_UserId == 0) Load(); return _User_Email; }
            set { _User_Email = value; }
        }
        private void Load()
        {
            var _context = new DataAggregatorContext("BaseClassAuth");
            var U = _context.UserViewAll.Where(w => w.Id == _User_guid.ToString()).Single();
            _User_Email = U.Email;
            _User_FullName = U.FullName;
            _UserId = U.UserId;
        }
        private void Load_Provizor()
        {
            using (var context = new DrugClassifierContext("BaseClassAuth"))
            {
                var US = context.UserSource.Where(w => w.UserId == _User_guid).Single();
                _PeriodId = US.PeriodId;
                _SourceId = US.SourceId;
            }
        }
        long _SourceId = 0;
        long _PeriodId = 0;
        public long PeriodId
        {
            get { if (_PeriodId == 0) Load_Provizor(); return _PeriodId; }
        }
        public long SourceId
        {
            get { if (_SourceId == 0) Load_Provizor(); return _SourceId; }
        }
    }

}