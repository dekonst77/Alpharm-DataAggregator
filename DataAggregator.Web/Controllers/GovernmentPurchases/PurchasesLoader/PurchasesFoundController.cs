using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Web.Models.GovernmentPurchases.DistributionWork;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases.PurchasesLoader
{

    [Authorize(Roles = "GManager")]
    public class PurchasesFoundController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~PurchasesFoundController()
        {
            _context.Dispose();
            //Dispose();
        }

        /// <summary>
        /// Получить данные
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GetData(FilterPurchasesJson filterPurchases)
        {

            var sqlQuery = @"   SELECT TOP 50000   
                                       [Id]
                                      ,[FzNumber]
                                      ,[Number]
                                      ,[Url]
                                      ,[Name]
                                      ,[SearchDate]                                    
                                      ,[Customer]
                                      ,[Method]
                                      ,[Stage]
                                      ,[Sum]                                     
                                      ,[PublishDate]
                                      ,[UpdateDate]
                                      ,[PurchaseClassId]
                                      ,[PurchaseClass]
                                      ,[PurchaseClassUser]
                                      ,[PurchaseClassDate]
                                      ,[PurchaseStage]
                                      ,[ErrorMessage]";


            if (filterPurchases.PurchaseClassId == 1 && filterPurchases.PurchaseClassUserId == null)
            {
                sqlQuery += "FROM [search].[SelectionPurchaseLinkView] WHERE";
            }
            else
            {
                sqlQuery += "FROM [search].[SelectionPurchaseLinkSetPurchaseClassView] WHERE";
            }

            StringBuilder sqlWhere = new StringBuilder();

            if (filterPurchases.DateBegin_Start != null)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                var start = DateTime.Parse(filterPurchases.DateBegin_Start);
                sqlWhere.Append(string.Format("DATEDIFF(DAY,'{0}' ,PublishDate) >= 0", start.ToString("yyyy-MM-dd")));
            }

            if (filterPurchases.DateBegin_End != null)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                var end = DateTime.Parse(filterPurchases.DateBegin_End);
                sqlWhere.Append(string.Format("DATEDIFF(DAY,'{0}' ,PublishDate) <= 0", end.ToString("yyyy-MM-dd")));
            }

            if (filterPurchases.PurchaseClassId.HasValue)
            {
                if (sqlWhere.Length > 0)
                    sqlWhere.Append(" and ");
                sqlWhere.Append(string.Format("PurchaseClassId = {0}", filterPurchases.PurchaseClassId.Value));
            }

            //Если задан конкретный пользователь
            if (filterPurchases.PurchaseClassUserId != null)
            {
                //Ищем назначенные этому пользотвалею
                sqlWhere.Append(string.Format(" and PurchaseClassUserId = '{0}'", filterPurchases.PurchaseClassUserId));
            }

            string fullQuery = String.Format("{0} {1}", sqlQuery, sqlWhere);

            var result = await _context.SelectionPurchaseLinkView.SqlQuery(fullQuery).ToListAsync();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result

            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузка справочника 
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Загрузка справочника 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetPurchaseClasses()
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                List<PurchaseClass> result = await context.PurchaseClass.ToListAsync();

                var jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
                return jsonNetResult;
            }
        }

        //Устанавливает выбранным элементам класс
        [HttpPost]
        public ActionResult SetPurchaseClass(List<long> ids, Byte purchaseClassId)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            var user = _context.User.FirstOrDefault(u => u.Id == userGuid.ToString());

            DateTime dateUpdate;

            if (user == null)
                throw new ApplicationException("user not found");

            using (var context = new GovernmentPurchasesContext(APP))
            {
                dateUpdate = context.SetPurchaseClass(ids, purchaseClassId, userGuid);
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new { UserFullName = user.Surname, Success = true, DateUpdate = dateUpdate }
            };

            return jsonNetResult;

        }
    }
}