using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.DistributionWork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager, GOperator")]
    public class ContractDistributionWorkController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~ContractDistributionWorkController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetUsers()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.User.ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetPurchaseClass()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.PurchaseClass.ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult SetAssignedContractToUser(List<long> purchasesId, string userId)
        {
            var purchase = _context.Purchase.Where(p => purchasesId.Contains(p.Id));

            if (purchase == null || purchase.Count() != purchasesId.Count)
                throw new ApplicationException("purchase not found");

            Guid? userGuid = string.IsNullOrEmpty(userId) ? (Guid?) null : new Guid(userId);

            purchase.ForEach(p => p.ContractAssignedToUserId = userGuid);

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult RemoveKK(List<long> purchasesId)
        {
            var purchases = _context.Purchase.Where(p => purchasesId.Contains(p.Id));

            if (purchases == null || purchases.Count() != purchasesId.Count)
                throw new ApplicationException("purchase not found");

            foreach (var purchase in purchases.ToList())
            {
                if (purchase.Lot != null)
                {
                    foreach (var lot in purchase.Lot)
                    {
                        _context.Contract.Where(c => c.LotId == lot.Id).ForEach(c => c.KK = 0);
                    }
                }
            }

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult GetPurchases(FilterPurchasesJson filterPurchases)
        {
            var sqlQuery = @"SELECT TOP 100000     [Id]
                                                  ,[Number]
                                                  ,[Name]
                                                  ,[DateBegin]
                                                  ,[DateEnd]
                                                  ,[PurchaseClassId]
                                                  ,[URL]
                                                  ,[PurchaseClassUserId]
                                                  ,[PurchaseClass]
                                                  ,[ContractAssignedToUserId]
                                                  ,[ContractAssignedToUser]
                                                  ,[PurchaseClassUser]
                                                  ,[StatusId]
                                                  ,[UserInWork]
                                                  ,[Sum]
                                                  ,[ContractCount]
                                                  ,[ContractKKCount],[ContractPDFCount]
                                                  ,[NatureName]
,[Supplier_Winner]
                        FROM [dbo].[ContractDistributionWork] WHERE";

            StringBuilder sqlWhere = new StringBuilder();

            if (filterPurchases.DateBegin_Start != null)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                var start = DateTime.Parse(filterPurchases.DateBegin_Start);
                sqlWhere.Append(string.Format("DATEDIFF(DAY,'{0}' ,DateBegin) >= 0", start.ToString("yyyy-MM-dd")));
              
            }

            if (filterPurchases.DateBegin_End != null)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                var end = DateTime.Parse(filterPurchases.DateBegin_End);
                sqlWhere.Append(string.Format("DATEDIFF(DAY,'{0}' ,DateBegin) <= 0", end.ToString("yyyy-MM-dd")));

            }

            if (filterPurchases.PurchaseClassId.HasValue)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                sqlWhere.Append(string.Format("PurchaseClassId = {0}", filterPurchases.PurchaseClassId.Value));
            }

            //Если стоит флаг - ищем назначено
            if (filterPurchases.IsAssignedToUser)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");

                //Если задан конкретный пользователь
                if (filterPurchases.AssignedToUserId != null)
                {
                    //Ищем назначенные этому пользотвалею
                    sqlWhere.Append(string.Format("ContractAssignedToUserId = '{0}'", filterPurchases.AssignedToUserId));
                }
                else
                {
                    //Ищем все назначенные
                    sqlWhere.Append("ContractAssignedToUserId is not null");
                }
            }
            else
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");

                //Ищем те которые не назначены
                sqlWhere.Append("ContractAssignedToUserId is null");
            }
            if (filterPurchases.isCheckTZ)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                sqlWhere.Append(@" [Id] in(SELECT        Purchase.Id
FROM            Purchase INNER JOIN
                         Lot ON Purchase.Id = Lot.PurchaseId INNER JOIN
                         Contract C ON Lot.Id = C.LotId
where C.ReestrNumber in(
select ReestrNumber from dbo.[contract_log_change] where value_new='Замена ТЗ обработки')
and C.id not in (select [ContractId] from ContractObjectReady))");
            }
            string fullQuery = String.Format("{0} {1}", sqlQuery, sqlWhere);

            _context.Database.CommandTimeout = 0;
            var result = _context.ContractDistributionWork.SqlQuery(fullQuery).ToList();


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result

            };

            return jsonNetResult;
        
        }
    }
}