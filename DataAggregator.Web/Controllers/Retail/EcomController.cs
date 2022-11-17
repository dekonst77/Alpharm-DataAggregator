using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.IO.Compression;
using DataAggregator.Domain.Model.Ecom;
using DataAggregator.Domain.Utils;
using System.Data;
using System.Threading.Tasks;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "Ecom")]
    public class EcomController : BaseController
    {
        public ActionResult Coefficients_Init()
        {
            try
            {
                var _context = new EcomContext(APP);
                //_context.Fill_Table_Coefficient_Default();
                var Periods = _context.RegionalCoefficients.Select(s => s.Period ).Distinct().OrderByDescending(o => o).ToList().Select(s => s.ToString("yyyy-MM"));
                ViewData["Periods"] = Periods;
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewData
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Coefficients_Search(string currentperiod)
        {
            try
            {
                DateTime Period = Convert.ToDateTime(currentperiod + "-15");
                var _context = new EcomContext(APP);
                _context.Database.CommandTimeout = 0;
                var RegionalCoefficients = _context.RegionalCoefficients.Where(w => w.Period == Period).OrderBy(o=>o.Region).ToList();
                var CoefficientsCount = _context.CoefficientsCount.Where(w => w.Period == Period).OrderBy(o => o.CountCategory).ToList();
                var CoefficientsPrice = _context.CoefficientsPrice.Where(w => w.Period == Period).OrderBy(o => o.PriceCategory).ToList();
                var Coefficients = _context.Coefficients_PivotView.Where(w => w.Period == Period).OrderBy(o => o.RegionCode).ToList();
                ViewData["RegionalCoefficients"] = RegionalCoefficients.ToList();
                ViewData["CoefficientsCount"] = CoefficientsCount.ToList();
                ViewData["CoefficientsPrice"] = CoefficientsPrice.ToList();
                ViewData["Coefficients"] = Coefficients.ToList();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewData
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult Coefficients_Save(ICollection<DataAggregator.Domain.Model.Ecom.RegionalCoefficients> RegionalCoefficients,
            ICollection<DataAggregator.Domain.Model.Ecom.CoefficientsCount> CoefficientsCount,
            ICollection<DataAggregator.Domain.Model.Ecom.CoefficientsPrice> CoefficientsPrice,
            ICollection<DataAggregator.Domain.Model.Ecom.Coefficients_PivotView> Coefficients)
        {
            try
            {
                var _context = new EcomContext(APP);
                if (RegionalCoefficients != null)
                    foreach (var item in RegionalCoefficients)
                    {
                        var UPD = _context.RegionalCoefficients.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode).Single();
                        UPD.RegCoeff = item.RegCoeff;
                    }

                if (CoefficientsCount != null)
                    foreach (var item in CoefficientsCount)
                    {
                        var UPD = _context.CoefficientsCount.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode && w.CountCategory == item.CountCategory).Single();
                        UPD.CountMax = item.CountMax;
                        UPD.CountMin = item.CountMin;
                    }

                if (CoefficientsPrice != null)
                    foreach (var item in CoefficientsPrice)
                    {
                        var UPD = _context.CoefficientsPrice.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode && w.PriceCategory == item.PriceCategory).Single();
                        UPD.PriceMin = item.PriceMin;
                        UPD.PriceMax = item.PriceMax;
                    }

                if (Coefficients != null)
                    foreach (var item in Coefficients)
                    {
                        var UPD_A = _context.Coefficients.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode &&
                        w.PriceCategory == item.PriceCategory && w.CountCategory == "A").Single();
                        UPD_A.Coefficient = item.CoefficientColsA;

                        var UPD_B = _context.Coefficients.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode &&
    w.PriceCategory == item.PriceCategory && w.CountCategory == "B").Single();
                        UPD_B.Coefficient = item.CoefficientColsB;

                        var UPD_C = _context.Coefficients.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode &&
    w.PriceCategory == item.PriceCategory && w.CountCategory == "C").Single();
                        UPD_C.Coefficient = item.CoefficientColsC;

                        var UPD_D = _context.Coefficients.Where(w => w.Period == item.Period && w.RegionCode == item.RegionCode &&
    w.PriceCategory == item.PriceCategory && w.CountCategory == "D").Single();
                        UPD_D.Coefficient = item.CoefficientColsD;
                    }
                _context.SaveChanges();
                _context.Table_Coefficient_Test_Min_Max();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = true
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public ActionResult Coefficients_Calc(string currentperiod)
        {
            try
            {
                DateTime Period = Convert.ToDateTime(currentperiod + "-15");
                var _context = new EcomContext(APP);
                _context.Database.CommandTimeout = 0;
                _context.EcomRun(Period);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewData
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Source_Calc(string currentperiod)
        {
            try
            {
                DateTime Period = Convert.ToDateTime(currentperiod + "-1");
                var _context = new EcomContext(APP);
                _context.Database.CommandTimeout = 0;
                await _context.EcomExportSourceRun(Period);
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = ViewData
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult Coefficients_from_Excel(IEnumerable<System.Web.HttpPostedFileBase> uploads, string currentperiod)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql3\Upload\Coefficients_from_Excel_" + User.Identity.GetUserId() + ".xlsx";
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);
            var _context = new EcomContext(APP);
            _context.Coefficients_from_Excel(User.Identity.GetUserId(), currentperiod);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResult() { Data = null}
            };
            return jsonNetResult;
        }
    }

}