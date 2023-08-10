using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.EtalonPrice;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.OFD
{
    public class EtalonPriceController : BaseController
    {
        private readonly OFDContext _context;
        private readonly Guid RobotUserId = new Guid("1fec04b1-9cff-4ae5-8f0a-d49ff5e1aa98");

        public EtalonPriceController()
        {
            _context = new OFDContext(APP);
        }

        ~EtalonPriceController()
        {
            _context.Dispose();
        }

        [HttpGet]
        public ActionResult GetCommentStatuses()
        {
            try
            {
                var result = _context.CommentStatuses.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult GetList(int year, int month, decimal? devPercent = null, string searchText = null, int? priceDiffDirection = null)
        {
            try
            {
                var result = _context.ViewData_SP(year, month, devPercent, searchText, priceDiffDirection).ToList();
                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ReloadAllData(int year, int month)
        {
            try
            {
                _context.Database.CommandTimeout = 10 * 60;
                _context.ReloadAllData_SP(year, month);

                var Data = new JsonResultData() { Data = null, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult SaveCommentStatuses(IEnumerable<Domain.Model.EtalonPrice.MainData> array)
        {
            try
            {
                if (array != null && array.Any())
                    using (_context)
                    {

                        foreach (var item in array)
                        {
                            var mainDataItem = _context.MainData.FirstOrDefault(x => x.Id == item.Id);
                            if (mainDataItem != null)
                            {
                                if (!string.IsNullOrEmpty(item.CommentStatusManual))
                                    mainDataItem.CommentStatusManual = item.CommentStatusManual;
                                mainDataItem.CommentStatusId = item.CommentStatusId;
                                mainDataItem.DateModified = DateTime.Now;
                                if (mainDataItem.UserId != RobotUserId)
                                    mainDataItem.UserId = new Guid(User.Identity.GetUserId());
                            }

                        }

                        _context.SaveChanges();
                    }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult TransferPrice(int year, int month, IEnumerable<Domain.Model.EtalonPrice.MainData> array) 
        {
            try
            {
                if (array != null && array.Any())
                {
                    var userId = new Guid(User.Identity.GetUserId());

                    string filename = $@"\\s-sql2\Upload\PriceTransfer_{userId}.xlsx";

                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    using (var xlsx = new ExcelPackage())
                    {
                        xlsx.Workbook.Worksheets.Add("Worksheet1");
                        xlsx.Workbook.Worksheets[0].Cells[1, 1].Value = "ClassifierId";
                        xlsx.Workbook.Worksheets[0].Cells[1, 2].Value = "Price";

                        int row = 2;
                        foreach (var item in array)
                        {
                            xlsx.Workbook.Worksheets[0].Cells[row, 1].Value = item.ClassifierId.ToString();
                            xlsx.Workbook.Worksheets[0].Cells[row, 2].Value = item.TransferPrice.ToString().Replace(".", ",");
                            row ++;
                        }

                        FileInfo excelFile = new FileInfo(filename);
                        xlsx.SaveAs(excelFile);
                        _context.EtaloPrice_Import_from_Excel(year * 100 + month, userId, filename);
                    }

                    var classifiers = array.Select(x => x.ClassifierId).ToArray();
                    using (var _ctx = new OFDContext(APP))
                    {
                        var data = from d in _ctx.MainData.Where(x => classifiers.Contains(x.ClassifierId)) 
                                   join u in _ctx.LinkedUserData
                                       on d.UserId equals new Guid(u.Id)
                                           select new 
                                           {
                                               ClassifierId = d.ClassifierId,
                                               TransferPrice = d.TransferPrice,
                                               DeviationPercent = d.DeviationPercent,
                                               PriceDiff = d.PriceDiff,
                                               DateModified = d.DateModified,
                                               UserName = u.Name
                                           };

                        return new JsonNetResult
                        {
                            Data = new JsonResult() 
                            { 

                                Data = data.ToArray() 
                            }
                        };
                    }
                }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        public ActionResult UploadFromExcel(int year, int month, HttpPostedFileBase file)
        {
            try
            {
                using (_context)
                {
                    var userId = new Guid(User.Identity.GetUserId());

                    string filename = $@"\\s-sql2\Upload\EtalonPriceUpload_{userId}.xlsx";

                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);

                    file.SaveAs(filename);

                    _context.EtaloPrice_Import_from_Excel(year * 100 + month, userId, filename);
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

        [HttpPost]
        public ActionResult GetSourceInfo(int year, int month, long classifierId)
        {
            try
            {
                _context.Database.CommandTimeout = 10 * 60;
                var result = _context.SourceInfoView_SP(year, month, classifierId).ToList();
                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SetForChecking(int? id)
        {
            try
            {
                if (id != null)
                    using (_context)
                    {
                        var mainDataItem = _context.MainData.FirstOrDefault(x => x.Id == id);
                        if (mainDataItem != null)
                        {
                            mainDataItem.ForChecking = true;
                            _context.SaveChanges();
                        }
                    }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}