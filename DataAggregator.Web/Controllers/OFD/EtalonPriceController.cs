using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.OFD;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.OFD
{
    public class EtalonPriceController : BaseController
    {
        private readonly OFDContext _context;

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
        public ActionResult GetList(int year, int month, decimal? devPercent = null, string searchText = null)
        {
            try
            {
                var result = _context.ViewData_SP(year, month, devPercent, searchText).ToList();
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
                _context.Database.CommandTimeout = 5 * 60;
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
                                mainDataItem.CommentStatusId = item.CommentStatusId;
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
        public ActionResult TransferPrice(IEnumerable<Domain.Model.EtalonPrice.MainData> array)
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
                                mainDataItem.TransferPrice = item.TransferPrice;
                                mainDataItem.DateModified = DateTime.Now;
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
    }
}