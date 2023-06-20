using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss")]
    public class SourceBrandBlackListController : BaseController
    {
        private RetailContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new RetailContext();
        }

        ~SourceBrandBlackListController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetList(int year, int month)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.SourceBrandBlackList.Where(x => x.Year == year && x.Month == month).ToArray()
            };
        }

        public ActionResult UploadFromExcel(int month, int year, HttpPostedFileBase file)
        {
            try
            {
                using (_context)
                {
                    string filename = @"\\s-sql2\Upload\SourceBrandBlackList_" + User.Identity.GetUserId() + ".xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);

                    _context.ImportSourceBrandBlackList_from_Excel(month, year, filename);
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonNetResult() { Data = null }
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}