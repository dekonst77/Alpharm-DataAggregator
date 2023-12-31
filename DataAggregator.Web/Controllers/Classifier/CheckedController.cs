﻿using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss,RManager")]
    public class CheckedController : BaseController
    {
        public ActionResult Checked_Init()
        {
            try
            {
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
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
        public ActionResult Checked_search(bool IsBrick, bool isOther, bool ToBlock)
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var result = _context.ClassifierInfo_GetReport(IsBrick, isOther);
                if (ToBlock)
                {
                    result = result.Where(w => w.Used == true && w.ToBlockUsed == true);
                }
                ViewData["Checkeds"] = result;
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult Checked_save(ICollection<ClassifierInfo_Report> array_UPD)
        {
            if (array_UPD == null)
                return BadRequest("Нет изменеий");

            try
            {
                var _context = new DrugClassifierContext(APP);
                var _etalon_context = new OFDContext(APP);
                if (array_UPD != null)
                    foreach (var item in array_UPD)
                    {
                        if (item.ci_comment == null)
                            item.ci_comment = "";
                        var UPD = _context.ClassifierInfo.Where(w => w.Id == item.Id).FirstOrDefault();
                        var ET_Price = _etalon_context.PriceCurrent.Where(w => w.ClassifierId == item.Id).FirstOrDefault();
                        var ET_Price_v2 = _etalon_context.PriceCurrent_v2.Where(w => w.ClassifierId == item.Id).FirstOrDefault();
                        if (ET_Price == null)
                        {
                            ET_Price = new Domain.Model.EtalonPrice.PriceCurrent
                            {
                                ClassifierId = (int)item.Id,
                                ForChecking = false,
                                PriceNew = item.PriceNew
                            };
                            _etalon_context.PriceCurrent.Add(ET_Price);
                        }
                        else
                        {
                            ET_Price.PriceNew = item.PriceNew;
                        }

                        if (ET_Price_v2 == null)
                        {
                            ET_Price_v2 = new Domain.Model.EtalonPrice.PriceCurrent_v2
                            {
                                ClassifierId = (int)item.Id,
                                ForChecking = false,
                                PriceNew = item.PriceNew
                            };
                            _etalon_context.PriceCurrent_v2.Add(ET_Price_v2);
                        }
                        else
                        {
                            ET_Price.PriceNew = item.PriceNew;
                        }


                        if (UPD == null)
                        {

                        }
                        else
                        {
                            UPD.ci_comment = item.ci_comment;
                            UPD.ToBlockUsed = item.ToBlockUsed;
                            UPD.ToSplitMnn = item.ToSplitMnn;
                            UPD.ToOFD = item.ToOFD;
                            UPD.ToRetail = item.ToRetail;
                            UPD.IsSTM = item.IsSTM;
                            UPD.ToSplitMnn_Signed = item.ToSplitMnn_Signed;
                            if (UPD.ProductionInfo != null && UPD.ProductionInfo.Used != item.Used && (User.IsInRole("ClassifierUsed")))
                            {
                                UPD.ProductionInfo.Used = item.Used;
                                if (item.Used == false && !string.IsNullOrEmpty(item.ci_comment))
                                {
                                    if (!string.IsNullOrEmpty(UPD.ProductionInfo.Comment))
                                    {
                                        UPD.ProductionInfo.Comment += " " + item.ci_comment;
                                    }
                                    else
                                    {
                                        UPD.ProductionInfo.Comment = item.ci_comment;
                                    }
                                }

                            }
                            if (UPD.GoodsProductionInfo != null && UPD.GoodsProductionInfo.Used != item.Used && (User.IsInRole("SBoss")))
                            {
                                UPD.GoodsProductionInfo.Used = item.Used;
                            }
                        }
                    }

                _etalon_context.SaveChanges();
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = array_UPD.Count, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult ExceptionListInit()
        {
            try
            {
                var _context = new DrugClassifierContext(APP);
                var result = _context.INNGroups.Select(item => new { item.Id, item.Description, item.IsException }).ToList();
                ViewData["ExceptionList"] = result;
                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult ExceptionListSave(ICollection<INNGroup> array_UPD)
        {
            if (array_UPD == null)
                return BadRequest("Нет входных данных");

            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    foreach (var item in array_UPD)
                    {
                        var UPD = _context.INNGroups.Find(item.Id);
                        UPD.IsException = item.IsException;
                    }
                    _context.SaveChanges();
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Множественная простановка всем "дробить по МНН"
        /// </summary>
        /// <param name="array_UPD">список Id классификаторов</param>
        /// <param name="value">дробить или снять дробление</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SplitByINN(IEnumerable<long> array_UPD, Boolean value)
        {
            if (array_UPD == null)
                return BadRequest("Нет входных данных");

            var ClassifireList = array_UPD.Select(item => item.ToString()).ToList();
            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    var result = _context.SplitByINN_SP(String.Join(",", ClassifireList), value);
                    ViewData["ClassifierCheckedRecords"] = result.ToArray();
                    var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResult() { Data = Data }
                    };
                    return jsonNetResult;
                }
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

        /// <summary>
        /// Проставить (снять) проверку на дробление по МНН, поле [ToSplitMnn_Signed]
        /// </summary>
        /// <param name="array_UPD">список Id классификаторов</param>
        /// <param name="value">поставить или снять проверку</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SplitByINN_Signed(IEnumerable<long> array_UPD, Boolean value)
        {
            if (array_UPD == null)
                return BadRequest("Нет входных данных");

            var ClassifireList = array_UPD.Select(item => item.ToString()).ToList();
            try
            {
                using (var _context = new DrugClassifierContext(APP))
                {
                    var result = _context.SplitByINN_Signed_SP(String.Join(",", ClassifireList), value);
                    ViewData["ClassifierCheckedRecords"] = result.ToArray();
                    var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                    JsonNetResult jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = new JsonResult() { Data = Data }
                    };
                    return jsonNetResult;
                }
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

        /// <summary>
        /// экспорт в Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileContentResult Checked_To_Excel()
        {
            using (var _context = new DrugClassifierContext(APP))
            {
                var result = _context.ClassifierInfoExcel_GetReport(false, false).ToList();

                Excel.Excel excel = new Excel.Excel();
                excel.Create();

                excel.InsertDataTable("Дробить по МНН", 1, 1, result, true, true, null);

                byte[] bb = excel.SaveAsByte();

                return File(bb, "application/vnd.ms-excel");
            }
        }

    }
}