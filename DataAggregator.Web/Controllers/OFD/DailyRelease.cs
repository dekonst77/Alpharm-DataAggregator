using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.OFD
{
    [Authorize(Roles = "OFD_View")]
    public class DailyRelease : BaseController
    {
        [HttpGet]
        public ActionResult InitPeriods()
        {
            return View();
        }
    }
}