using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class CalcRunnerController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~CalcRunnerController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult CreateExternal()
        {

            string ret = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, "GovernmentPurchases_Start_Full", Domain.Model.ControlALG.ControlALG.JobStartAction.start, new Guid(User.Identity.GetUserId()));
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
            };
        }


        /// <summary>
        /// Запуск создания базы гос сегмента
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "GCreateExternalShipment,Admin")]
        public ActionResult CreateExternalShipment()
        {
            string ret = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, "GovernmentPurchases_CreateExternalShipmentDatabase", Domain.Model.ControlALG.ControlALG.JobStartAction.start, new Guid(User.Identity.GetUserId()));
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
            };
        }

        
        [HttpPost]
        public ActionResult SetChecked(bool value)
        {
            var databaseChecked = _context.DataBaseChecked.Single(db => db.Database == "GovernmentSegmentShipment");

            if (databaseChecked.Created)
            {
                databaseChecked.Checked = value;
                databaseChecked.CheckedDate = DateTime.Now;
            }

            _context.SaveChanges();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = databaseChecked.Checked
            };
        }

        [HttpPost]
        public ActionResult GetExternalGovernmentPurchasesStatus()
        {
            var transfer = _context.DataBaseTransfer.OrderByDescending(i => i.Id).First();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new { Step = transfer.Step, Date = transfer.Date.ToString("dd.MM.yyyy HH:mm:ss") }
            };
        }
        [HttpPost]
        public ActionResult CreateExternalShipmentDatabase_startNight()
        {
            _context.Database.ExecuteSqlCommand(@"update [ControlALG].[dbo].[Setting] set value= case when value='true' then 'false' else 'true' end where id='CreateExternalShipmentDatabase_startNight'");

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = { }
            };
        }

        [HttpPost]
        public ActionResult GetExternalGovernmentPurchasesShipmentStatus()
        {
            var shipmentLog = _context.CreateExternalShipmentLog.OrderByDescending(i => i.Id).FirstOrDefault();
            var databaseChecked = _context.DataBaseChecked.Single(db => db.Database == "GovernmentSegmentShipment");
          
            
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = shipmentLog != null ? new { Step = shipmentLog.Step, Date = shipmentLog.Date.ToString("dd.MM.yyyy HH:mm:ss"), Created = databaseChecked.Created, Checked = databaseChecked.Checked } :
                                             new { Step = "еще не запускалась", Date = "", Created = false, Checked  = false}
            };
        }
        [HttpPost]
        public ActionResult GetStatuses()
        {
            var CreateExternalShipmentDatabase_startNight_value = _context.Database.SqlQuery<string>("select value from [ControlALG].[dbo].[Setting] where id='CreateExternalShipmentDatabase_startNight'").FirstOrDefault();

            if (CreateExternalShipmentDatabase_startNight_value.ToLower() == "true")
            {
                CreateExternalShipmentDatabase_startNight_value = "включён";
            }
            else
                CreateExternalShipmentDatabase_startNight_value = "выключён";

            string GovernmentPurchases_Start_Full = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, "GovernmentPurchases_Start_Full", Domain.Model.ControlALG.ControlALG.JobStartAction.info);
            string GovernmentPurchases_CreateExternalShipmentDatabase = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, "GovernmentPurchases_CreateExternalShipmentDatabase", Domain.Model.ControlALG.ControlALG.JobStartAction.info, new Guid(User.Identity.GetUserId()));

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new { 
                    CreateExternalShipmentDatabase_startNight_value = CreateExternalShipmentDatabase_startNight_value ,
                    GovernmentPurchases_Start_Full = GovernmentPurchases_Start_Full,
                    GovernmentPurchases_CreateExternalShipmentDatabase = GovernmentPurchases_CreateExternalShipmentDatabase
                }
            };
        }

        [HttpPost]
        public ActionResult GetCalcStatus()
        {
            _context.Database.CommandTimeout = 0;

            List<string> locks = new List<string>();

            try
            {

                //Получаем список всех локов
                locks = _context.Database.SqlQuery<string>(@"SELECT resource_description
                                               FROM sys.dm_tran_locks with(nolock)
                                               WHERE resource_type = 'APPLICATION' and resource_description like '%GP%'").ToList();
                  
            }
            catch
            {
                locks.Add("GP_all_Lock");
            }


            var calcStatus = new Dictionary<string, int>
            {
               
                {
                    "CalcAveragePrice",
                    locks.Count(l => l.Contains("GP_CalcAveragePriceAllDb_Lock"))
                    //_context.Database.SqlQuery<int>(
                    //    "select count(*) from sys.dm_tran_locks with(nolock) where resource_description like '%GP_CalcAveragePriceAllDb_Lock%'").Single()
                },
                {
                    "CopyToCalculatedContractObject",
                      locks.Count(l => l.Contains("GP_CopyToCalcContrObj_Lock"))
                    //_context.Database.SqlQuery<int>(
                    //    "select count(*) from sys.dm_tran_locks with(nolock) where resource_description like '%GP_CopyToCalcContrObj_Lock%'")
                    //    .Single()
                },
                {
                    "CopyToCalculatedPurchaseObject",
                      locks.Count(l => l.Contains("GP_CopyToCalcPurchObj_Lock"))
                    //_context.Database.SqlQuery<int>(
                    //    "select count(*) from sys.dm_tran_locks with(nolock) where resource_description like '%GP_CopyToCalcPurchObj_Lock%'")
                    //    .Single()
                },
                {
                    "CreateExternalDB",
                      locks.Count(l => l.Contains("GP_CreateExternalDB"))
                    //_context.Database.SqlQuery<int>(
                    //    "select count(*) from sys.dm_tran_locks with(nolock) where resource_description like '%GP_CreateExternalDB%'")
                    //    .Single()
                },
                {
                    "CreateExternal", Models.GovernmentPurchases.CalcRunner.CreateExternalGovernmentPurchases.Instance.IsRunning ? 1 : 0
                    
                },
                {
                    "CreateExternalShipment", 
                      locks.Count(l => l.Contains("GP_CreateExternalShipment"))
                    //_context.Database.SqlQuery<int>(
                    //    "select count(*) from sys.dm_tran_locks with(nolock) where resource_description like '%GP_CreateExternalShipmentDatabase_Lock%'")
                    //    .Single()
                },
                {
                    "GP_all_Lock",
                    locks.Count(l => l.Contains("GP_all_Lock"))
                },
                {
                    "QlikJobDisabled",
                    TimeInInterval(8,45,10,15) || TimeInInterval(14,30,16,45) ? 0 : 1 
                }

            };

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = calcStatus
            };
        }

        /// <summary>
        /// Признак, что текущее время попадает в указанный интервал
        /// </summary>
        private static bool TimeInInterval(int hourStart, int minuteStart, int hourEnd, int minuteEnd)
        {

            TimeSpan start = new TimeSpan(hourStart, minuteStart, 0); 
            TimeSpan end = new TimeSpan(hourEnd, minuteEnd, 0); 
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now >= start) && (now <= end))
                return true;

            return false;
        }

        [HttpPost]
        public ActionResult QlikJob()//переложить гс с тестового клика в клиентский
        {
            _context.Database.ExecuteSqlCommand(@"EXEC [MSK-AF-S-DWH04.QLIK.LOCAL\ALPHARM_DWH].msdb.dbo.sp_start_job N'ETL_GOS_Full_Process'");
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ""
            }; ;
        }

        [HttpGet]
        public ActionResult Action()
        {
            ViewBag.GovernmentPurchases_ToProvizor_status = DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, "GovernmentPurchases_ToProvizor", Domain.Model.ControlALG.ControlALG.JobStartAction.info, new Guid(User.Identity.GetUserId())).Replace("\r\n", @"<br />");
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewBag
            };
            return jsonNetResult;
        }
        [HttpPost]
        [Authorize(Roles = "GCreateExternalShipment,Admin")]
        public ActionResult Action(string name)
        {
           // using (var _ofd_context = new OFDContext(APP))
           // {
                switch (name)
                {
                    case "GovernmentPurchases_ToProvizor":
                        DataAggregator.Domain.Model.ControlALG.ControlALG.Start_Job(_context, name, Domain.Model.ControlALG.ControlALG.JobStartAction.start, new Guid(User.Identity.GetUserId()));
                        break;
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = new JsonResult() { Data = "", count = 0, status = "ок", Success = true }
                };
                return jsonNetResult;
           // }
        }
        public class JsonResult
        {
            public object Data { get; set; }
            public object Data2 { get; set; }
            public int count { get; set; }
            public string status { get; set; }
            public bool Success { get; set; }
        }
    }
}