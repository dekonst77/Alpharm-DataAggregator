using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierRxOtc;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class ClassifierRxOtcController : BaseController
    {
        private readonly DrugClassifierContext _context;

        public ClassifierRxOtcController()
        {
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Init(bool Used, bool Excluded)
        {
            try
            {
                IEnumerable<LoadClassifierRxOtc_SP_Result> result = _context.LoadClassifierRxOtc_SP_Result(Used, Excluded).ToList();
                ViewData["RxOtc"] = result;

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

        /// <summary>
        /// Сохранение записей
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(ICollection<LoadClassifierRxOtc_SP_Result> array)
        {
            List<ClassifierRxOtc> records = new List<ClassifierRxOtc>();

            array.ForEach(item =>
            {
                ClassifierRxOtc record = _context.ClassifierRxOtc.Find(item.ClassifierInfoId);

                if (record == null)
                {
                    record = new ClassifierRxOtc()
                    {
                        Classifierid = item.ClassifierInfoId,
                        IsRx = item.Rx,
                        IsChecked = item.IsChecked,
                        IsException = item.IsException ?? false,
                        Comment = item.Comment
                    };
                    _context.ClassifierRxOtc.Add(record);
                }
                else
                {
                    record.IsRx = item.Rx;
                    record.IsChecked = item.IsChecked;
                    record.IsException = item.IsException.Value;
                    record.Comment = item.Comment;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<ClassifierRxOtc>(record).State = EntityState.Detached;
                    records.Add(_context.ClassifierRxOtc.Find(item.ClassifierInfoId));
                }
            });

            ViewData["ClassifierRxOtcRecord"] = records;

            var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = Data }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult SetRx(ICollection<LoadClassifierRxOtc_SP_Result> array)
        {
            List<ClassifierRxOtc> records = new List<ClassifierRxOtc>();

            array.ForEach(item =>
            {
                ClassifierRxOtc record = _context.ClassifierRxOtc.Find(item.ClassifierInfoId);

                if (record == null)
                {
                    record = new ClassifierRxOtc()
                    {
                        Classifierid = item.ClassifierInfoId,
                        IsRx = true,
                        // IsException = true
                    };
                    _context.ClassifierRxOtc.Add(record);
                }
                else
                {
                    record.IsRx = true;
                    //    record.IsException = true;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<ClassifierRxOtc>(record).State = EntityState.Detached;
                    records.Add(_context.ClassifierRxOtc.Find(item.ClassifierInfoId));
                }
            });

            ViewData["ClassifierRxOtcRecord"] = records;

            var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = Data }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult SetOtc(ICollection<LoadClassifierRxOtc_SP_Result> array)
        {
            List<ClassifierRxOtc> records = new List<ClassifierRxOtc>();

            array.ForEach(item =>
            {
                ClassifierRxOtc record = _context.ClassifierRxOtc.Find(item.ClassifierInfoId);

                if (record == null)
                {
                    record = new ClassifierRxOtc()
                    {
                        Classifierid = item.ClassifierInfoId,
                        IsRx = false,
                        //    IsException = true
                    };
                    _context.ClassifierRxOtc.Add(record);
                }
                else
                {
                    record.IsRx = false;
                    //  record.IsException = true;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<ClassifierRxOtc>(record).State = EntityState.Detached;
                    records.Add(_context.ClassifierRxOtc.Find(item.ClassifierInfoId));
                }
            });

            ViewData["ClassifierRxOtcRecord"] = records;

            var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = Data }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult SetChecked(ICollection<LoadClassifierRxOtc_SP_Result> array)
        {
            List<ClassifierRxOtc> records = new List<ClassifierRxOtc>();

            array.ForEach(item =>
            {
                ClassifierRxOtc record = _context.ClassifierRxOtc.Find(item.ClassifierInfoId);

                if (record == null)
                {
                    record = new ClassifierRxOtc()
                    {
                        Classifierid = item.ClassifierInfoId,
                        IsChecked = true
                    };
                    _context.ClassifierRxOtc.Add(record);
                }
                else
                {
                    record.IsChecked = true;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<ClassifierRxOtc>(record).State = EntityState.Detached;
                    records.Add(_context.ClassifierRxOtc.Find(item.ClassifierInfoId));
                }
            });

            ViewData["ClassifierRxOtcRecord"] = records;

            var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = Data }
            };
            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult SetException(ICollection<LoadClassifierRxOtc_SP_Result> array)
        {
            List<ClassifierRxOtc> records = new List<ClassifierRxOtc>();

            array.ForEach(item =>
            {
                ClassifierRxOtc record = _context.ClassifierRxOtc.Find(item.ClassifierInfoId);

                if (record == null)
                {
                    record = new ClassifierRxOtc()
                    {
                        Classifierid = item.ClassifierInfoId,
                        IsException = true
                    };
                    _context.ClassifierRxOtc.Add(record);
                }
                else
                {
                    record.IsException = true;
                }

                if (_context.SaveChanges() > 0)
                {
                    _context.Entry<ClassifierRxOtc>(record).State = EntityState.Detached;
                    records.Add(_context.ClassifierRxOtc.Find(item.ClassifierInfoId));
                }
            });

            ViewData["ClassifierRxOtcRecord"] = records;

            var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = Data }
            };
            return jsonNetResult;
        }

        /// <summary>
        /// История изменеий Rx, Otc
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadHistory(HistoryFilter filter)
        {
            try
            {
                var history = _context.ClassifierRxOtcLog.Where(r => r.ClassifierId == filter.ClassifierId).OrderBy(r => r.When).ToList();
                return ReturnData(history);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

    }
}