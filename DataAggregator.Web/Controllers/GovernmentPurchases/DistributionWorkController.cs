using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.DistributionWork;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager, GOperator")]
    public class DistributionWorkController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~DistributionWorkController()
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
        public ActionResult SetAssignedToUser(List<long> purchasesId, string userId)
        {
            var purchase = _context.Purchase.Where(p => purchasesId.Contains(p.Id));

            if (purchase == null || purchase.Count() != purchasesId.Count)
                throw new ApplicationException("purchase not found");

            Guid? userGuid = string.IsNullOrEmpty(userId) ? (Guid?) null : new Guid(userId);

            purchase.ForEach(p => p.AssignedToUserId = userGuid);

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult SetPurchaseClass(List<long> purchasesId, Byte purchaseClassId)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            var user = _context.User.FirstOrDefault(u => u.Id == userGuid.ToString());

            if(user == null)
                throw new ApplicationException("user not found");

            var purchase = _context.Purchase.Where(p => purchasesId.Contains(p.Id));

            if (purchase == null || purchase.Count() != purchasesId.Count)
                throw new ApplicationException("purchase not found");

            purchase.ForEach(p =>
            {
                p.PurchaseClassId = purchaseClassId;
                p.PurchaseClassUserId = userGuid;
            });
            
            _context.SaveChanges();



            return Json(new { UserFullName = user.Surname, Success = true });
        }

        // TODO:
        //~ в форме только не измененные операторами
        //~ польз - только не назначенные
        //+ если раздел не выбран - только пустые
        //+ по датам можно ничего не выбирать

        // добавить фамилию
        // возможность посмотреть все розданные (все пользователи)
        // поля как на 2 странице
        [HttpPost]
        public ActionResult GetPurchases(FilterPurchasesJson filterPurchases)
        {
            _context.Database.CommandTimeout = 0;
            var result = _context.DistributionWork.Select(d => d);
            if (!filterPurchases.withPtotokol)
            {
                result = result.Where(w=>w.ExistsNature==false || w.ExistsDeliveryTimeInfo == false || w.ExistsLotFunding == false || w.ExistsPurchaseObject == false);
            }

            if (filterPurchases.DateBegin_Start != null)
            {
                var start = DateTime.Parse(filterPurchases.DateBegin_Start);
                result = result.Where(d => d.DateBegin >= start);
            }

            if (filterPurchases.DateBegin_End != null)
            {
                var end = DateTime.Parse(filterPurchases.DateBegin_End).AddDays(1);
                result = result.Where(d => d.DateBegin <= end);
            }

            if (filterPurchases.DateEnd_Start != null)
            {
                var start = DateTime.Parse(filterPurchases.DateEnd_Start);
                result = result.Where(d => d.DateEnd >= start);
            }

            if (filterPurchases.DateEnd_End != null)
            {
                var end = DateTime.Parse(filterPurchases.DateEnd_End).AddDays(1);
                result = result.Where(d => d.DateEnd <= end);
            }

            if (filterPurchases.PurchaseDateCreate_Start != null)
            {
                var start = DateTime.Parse(filterPurchases.PurchaseDateCreate_Start);
                result = result.Where(d => d.PurchaseDateCreate >= start);
            }

            if (filterPurchases.PurchaseDateCreate_End != null)
            {
                var end = DateTime.Parse(filterPurchases.PurchaseDateCreate_End).AddDays(1);
                result = result.Where(d => d.PurchaseDateCreate <= end);
            }

            if (filterPurchases.PurchaseClassId>0)
            {
                result = result.Where(d => d.PurchaseClassId == (Byte)filterPurchases.PurchaseClassId);
            }

            //Если стоит флаг - ищем назначено
            if (filterPurchases.IsAssignedToUser)
            {
                //Если задан конкретный пользователь
                if (filterPurchases.AssignedToUserId != null)
                {
                    //Ищем назначенные этому пользотвалею
                    result = result.Where(d => d.AssignedToUserId == filterPurchases.AssignedToUserId);
                }
                else
                {
                    //Ищем все назначенные
                    result = result.Where(d => d.AssignedToUserId != null);
                }
            }
            else
            {
                //Ищем те которые не назначены
                result = result.Where(d => d.AssignedToUserId == null);
            }
            
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()

            };

            return jsonNetResult;
        }
    }
}