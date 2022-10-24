using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;


namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class ObjectsToObjectsReadyController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~ObjectsToObjectsReadyController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult LoadLogs()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.AutomaticProcessesLogView.OrderByDescending(a => a.DateStart).Take(100).ToList()
            };

            return jsonNetResult;
        }


        [HttpPost]
        public void StartTransfer(Models.GovernmentPurchases.GovernmentPurchases.ObjectsTransferJson filterTransfer)
        {
            if (filterTransfer != null)
            {
                var userGuid = new Guid(User.Identity.GetUserId());

                DateTime dateStart = filterTransfer.DateStart.Date;
                DateTime dateEnd = filterTransfer.DateEnd.Date;
                bool transferContracts = filterTransfer.TransferContracts;
                bool transferObjects = filterTransfer.TransferObjects;

                var dateStartParam = new SqlParameter("dateStart", SqlDbType.DateTime);
                dateStartParam.Value = dateStart;
                var dateEndParam = new SqlParameter("dateEnd", SqlDbType.DateTime);
                dateEndParam.Value = dateEnd.AddDays(1);//чтобы выбрать всё до конца суток
                var userIdParam = new SqlParameter("userId", SqlDbType.UniqueIdentifier);
                userIdParam.Value = userGuid;

                _context.Database.CommandTimeout = 0;

                if (transferContracts && transferObjects)
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "exec ContractObjectsToContractObjectsReady @dateStart, @dateEnd, @userId; exec ObjectsToObjectsReady @dateStart, @dateEnd, @userId", dateStartParam, dateEndParam, userIdParam);

                if (!transferContracts && transferObjects)
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "exec ObjectsToObjectsReady @dateStart, @dateEnd, @userId", dateStartParam, dateEndParam, userIdParam);

                if (transferContracts && !transferObjects)
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "exec ContractObjectsToContractObjectsReady @dateStart, @dateEnd, @userId", dateStartParam, dateEndParam, userIdParam);
            }
        }
    }
}