using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager, GOperator")]
    public class DeliveryTimeController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~DeliveryTimeController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetDataByFilter(filtersController.filterWhere filter)
        {
            var result = new Dictionary<string, object>();
            var userGuid = new Guid(User.Identity.GetUserId());

            string query = "select top 50 PurchaseId from [dbo].[DeliveryTimeSetView] " + filter.Where("[PurchaseId]", "[LotId]", "");
            string query2 = "select * from [dbo].[DeliveryTimeSetView] Where PurchaseId " + _context.Lock(true, userGuid, "DeliveryTime", query, "PurchaseId");
            _context.Database.CommandTimeout = 0;
            var viewData = _context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.View.DeliveryTimeSetView>(query2);

            result.Add("data", viewData.ToList());
            result.Add("count", viewData.Count());

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
            return jsonNetResult;
        }

        public class ListItem
        { 
            public long code { get; set; }
            public string status { get; set; }
        }
        [HttpPost]
        public ActionResult InitMiniClassifiers()
        {
            var ret_dtp = _context.DeliveryTimePeriod.Select(s => new ListItem { code = s.Id, status = s.Name }).OrderBy(o => o.code).ToList();

            ret_dtp.Add(new ListItem() { code=0,status="пусто"});

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret_dtp
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult ResetLock(string typeLock)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            _context.Lock(false, userGuid, typeLock, "", "");

            return null;
        }
        [HttpPost]
        public ActionResult Save(ICollection<DataAggregator.Domain.Model.GovernmentPurchases.View.DeliveryTimeSetView> array)
        {
            var userGuid = new Guid(User.Identity.GetUserId());
            try
            {
                foreach (var item in array)
                {
                    //var purchase = _context.Purchase.Where(w => w.Id == item.PurchaseId).Single();
                    DataAggregator.Domain.Model.GovernmentPurchases.DeliveryTimeInfo DTI = null;
                    if (item.idDTI != null && item.idDTI != 0)
                    {
                        long idDTI = Math.Abs((long)item.idDTI);
                        DTI = _context.DeliveryTimeInfo.Where(w => w.Id == idDTI).Single();
                        if (item.idDTI < 0)
                        {//уделение данных
                            //_context.DeliveryTimeInfo.Remove(DTI);
                            continue;
                        }
                        else
                        {//Обновление данных
                            DTI.Count = item.Count;
                            DTI.DateStart = (DateTime)item.DateStartDTI;
                            DTI.DateEnd = (DateTime)item.DateEndDTI;
                            DTI.DeliveryTimePeriodId = (Byte)item.DeliveryTimePeriodId;
                            DTI.LastChangedUserIdDTI = userGuid;
                            DTI.LastChangedDateDTI = DateTime.Now;
                            continue;
                        }
                    }
                    if (item.DateEndDTI != null && (item.idDTI == null || item.idDTI == 0))
                    {//Добавление данных
                        _context.DeliveryTimeInfo.Add(new Domain.Model.GovernmentPurchases.DeliveryTimeInfo()
                        {
                            PurchaseId = item.PurchaseId,
                            Count = item.Count,
                            DateStart = (DateTime)item.DateStartDTI,
                            DateEnd = (DateTime)item.DateEndDTI,
                            DeliveryTimePeriodId = (Byte)item.DeliveryTimePeriodId,
                            LastChangedUserIdDTI = userGuid,
                            LastChangedDateDTI = DateTime.Now
                        });
                    }
                }


                _context.SaveChanges();

                string query2 = "select * from [dbo].[DeliveryTimeSetView] Where PurchaseId " + string.Format(" IN (select Id from [dbo].[Locks] where [typeLock]='{1}' and [UserId]='{0}')", userGuid, "DeliveryTime");
                var ret = _context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.View.DeliveryTimeSetView>(query2);

                var result = new Dictionary<string, object>();
                result.Add("data", ret);
                result.Add("count", ret.Count());
                result.Add("Success", true);
                
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
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
        public class JsonResult
        {
            public object Data { get; set; }
            public int count { get; set; }
            public string status { get; set; }
            public bool Success { get; set; }
        }
    }
}