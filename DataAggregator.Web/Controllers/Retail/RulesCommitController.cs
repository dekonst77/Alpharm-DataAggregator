﻿using DataAggregator.Domain.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class RulesCommitController : BaseController
    {
        public static DateTime? PeriodFrom { get; set; }
        public static DateTime? PeriodTo { get; set; }
        private readonly RetailCalculationContext _context;

        public RulesCommitController()
        {
            _context = new RetailCalculationContext();
        }

        ~RulesCommitController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult Log_search(DateTime PeriodFrom, DateTime PeriodTo)
        {
            try
            {
                object data = null;

                int YearMonthFrom = PeriodFrom.Year * 100 + PeriodFrom.Month;
                int YearMonthTo = PeriodTo.Year * 100 + PeriodTo.Month;

                using (_context)
                {
                    data = _context.CalculationLog.Where(x => x.Step.Contains("CommitCalculatedData")
                            && (x.Year * 100 + x.Month) >= YearMonthFrom
                            && (x.Year * 100 + x.Month) <= YearMonthTo
                         )
                        .OrderByDescending(x => x.Id).ToList();
                }

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonNetResult() { Data = data }
                };
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public ActionResult RunProcess(DateTime PeriodFrom, DateTime PeriodTo)
        {
            try
            {
                using (_context)
                {
                    _context.RulesCommit(PeriodFrom, PeriodTo);

                    var jsonNetResult = new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = true
                    };

                    return jsonNetResult;
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}