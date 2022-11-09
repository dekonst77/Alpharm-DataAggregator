using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SPharmacist")]
    public class BlisterBlockController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public BlisterBlockController()
        {
            _context = new DrugClassifierContext(APP);
        }

        ~BlisterBlockController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult BlisterBlockView()
        {
            try
            {
                var result = _context.BlisterBlockView.ToList();
                ViewData["BlisterBlock"] = result;
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult GetPrimaryPacking(int ClassifierId)
        {
            try
            {
                var result = _context.ClassifierPacking.Where(t => t.ClassifierId == ClassifierId).OrderBy(t => t.CountPrimaryPacking).Select(t => new { value = t.Id, label = t.CountPrimaryPacking }).ToList();

                JsonResult jsonResult = new JsonResult
                {
                    Data = new JsonResult() { Data = result }
                };
                return jsonResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult SaveField(Int64 ClassifierId, string FieldName, string newValue)
        {
            try
            {
                BlisterBlockView record = _context.BlisterBlockView.Find(ClassifierId);

                switch (FieldName)
                {
                    case "ClassifierPackingId":
                        record.ClassifierPackingId = int.Parse(newValue);
                        break;
                    case "Comment":
                        record.Comment = newValue;
                        break;
                    case "IsExist":
                        record.IsExist = bool.Parse(newValue);
                        break;
                    default:
                        break;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<BlisterBlockView>(record).State = EntityState.Detached;
                    record = _context.BlisterBlockView.Find(ClassifierId);
                }

                ViewData["BlisterBlockRecord"] = record;


                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

        [HttpPost]
        public ActionResult Save(ICollection<BlisterBlockView> array)
        {
            try
            {
                List<BlisterBlockView> records = new List<BlisterBlockView>();

                foreach (var item in array)
                {
                    BlisterBlockView record = _context.BlisterBlockView.Find(item.ClassifierId);
                    record.ClassifierPackingId = item.ClassifierPackingId;
                    record.Comment = item.Comment;
                    record.IsExist = item.IsExist;

                    if (_context.SaveChanges() > 0)
                    {
                        _context.Entry<BlisterBlockView>(record).State = EntityState.Detached;
                        records.Add(_context.BlisterBlockView.Find(item.ClassifierId));
                    }
                }

                ViewData["BlisterBlockRecord"] = records;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = Data }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

    }
}