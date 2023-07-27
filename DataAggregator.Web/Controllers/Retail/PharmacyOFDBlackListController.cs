using DataAggregator.Domain.BulkInsert;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
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
    public class PharmacyOFDBlackListController : BaseController
    {
        private RetailContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new RetailContext();
        }

        ~PharmacyOFDBlackListController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetList(int year, int month)
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.TargetPharmacyOFDBlackList.Where(x => x.Year == year && x.Month == month).ToList()
            };
        }

        public ActionResult UploadFromExcel(int month, int year, HttpPostedFileBase file)
        {
            try
            {
                var fileContents = new List<TargetPharmacyOFDBlackList>();
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using (var xlsx = new ExcelPackage(file.InputStream))
                {
                    var sheet = xlsx.Workbook.Worksheets[0];

                    if (!sheet.Cells[1, 1].Text.Equals("PharmacyId"))
                    {
                        throw new Exception("Некорректное содержимое файла");
                    }

                    for (var i = 2; i <= sheet.Dimension.End.Row; i++)
                    {
                        fileContents.Add(new TargetPharmacyOFDBlackList
                        {
                            TargetPharmacyId = sheet.Cells[i, 1].GetValue<long>(),
                            Year = year,
                            Month = month
                        });
                    }
                }

                if (fileContents.Any())
                {
                    _context.BulkInsert(fileContents);
                    _context.SaveChanges();
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