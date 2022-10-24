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
    public class MassFixesDataController : BaseController
    {

        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~MassFixesDataController()
        {
            _context.Dispose();
        }
        
        [HttpPost]
        public ActionResult GetDataByFilter(filtersController.filterWhere filter,bool level)
        {
          
            var result = new Dictionary<string, object>();

            string query = @"SELECT   top 50000     p.Id AS PurchaseId, p.Number AS PurchaseNumber, l.Id AS LotId, l.Number AS LotNumber, l.Sum as LotSum, p.Name AS PurchaseName, p.DateBegin, p.DateEnd, 
lt.Name AS LawTypeName, nat.Name AS NatureName, cat.Name AS CategoryName,
                          l.SourceOfFinancing, lf.FundingNames, p.DeliveryTime, dtp.DeliveryTimePeriod_text, p.URL, ls.Name AS LotStatusName, 
customer.FullName AS CustomerFullName, customer.ShortName AS CustomerShortName, 
                         customerRegion.FederationSubject AS CustomerFederationSubject, customerOrganizationType.Name AS CustomerOrganizationType, customer.INN AS CustomerINN, customer.OutId AS CustomerId, 
                         receiver.FullName AS ReceiverFullName, receiver.ShortName AS ReceiverShortName, receiverRegion.FederationSubject AS ReceiverFederationSubject, receiverOrganizationType.Name AS ReceiverOrganizationType, 
                         receiver.INN AS ReceiverINN, receiver.OutId AS ReceiverId, 'слишком долго ждать' AS CustomerSource, pay_p.KBKs, pay_c.KBKs AS contract_KBKs, pc.Name AS PurchaseClassName, U1.UserName AS LastChangedUser_Purchase,
                          U2.UserName AS LastChangedUser_Lot, U3.UserName AS LastChangedUser_PurchaseObjectReady, c.Id AS contractId, por.Id AS porID, p.Comment, nat2.Name AS Nature_L2Name
FROM            dbo.Purchase AS p(nolock) inner join
dbo.Lot AS l(nolock) ON l.PurchaseId = p.Id
LEFT JOIN dbo.Contract AS c(nolock) ON c.LotId = l.Id
LEFT JOIN dbo.PurchaseObjectReady AS por(nolock) ON por.LotId = l.Id
LEFT JOIN dbo.Nature_L2 AS nat2(nolock) ON p.Nature_L2Id = nat2.Id LEFT JOIN
                         dbo.Nature AS nat(nolock) ON nat.Id = p.NatureId LEFT JOIN
                         dbo.LawType AS lt(nolock) ON p.LawTypeId = lt.Id LEFT JOIN
                         dbo.PurchaseClass AS pc(nolock) ON pc.Id = p.PurchaseClassId LEFT JOIN
                         dbo.OrganizationOut AS customer(nolock) ON customer.Id = p.CustomerId LEFT JOIN
                         dbo.OrganizationType AS customerOrganizationType(nolock) ON customerOrganizationType.Id = customer.OrganizationTypeId LEFT JOIN
                         dbo.Region AS customerRegion(nolock) ON customerRegion.Id = customer.RegionId LEFT JOIN
                         dbo.Category AS cat(nolock) ON nat.CategoryId = cat.Id LEFT JOIN
                         dbo.OrganizationOut AS receiver(nolock) ON receiver.Id = por.ReceiverId LEFT JOIN
                         dbo.OrganizationType AS receiverOrganizationType(nolock) ON receiverOrganizationType.Id = receiver.OrganizationTypeId LEFT JOIN
                         dbo.Region AS receiverRegion(nolock) ON receiverRegion.Id = receiver.RegionId LEFT JOIN
                         dbo.LotFundingMergeLotView(nolock) AS lf ON lf.LotId = l.Id  LEFT JOIN
                         dbo.SupplierResult AS sr(nolock) ON sr.LotId = l.Id LEFT JOIN
                         dbo.LotStatus AS ls(nolock) ON ls.Id = sr.LotStatusId LEFT JOIN
                         dbo.DeliveryTimeInfoMergePurchaseView AS dtp(nolock) ON dtp.PurchaseId = p.Id   LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U1(nolock) ON p.LastChangedUserId = U1.Id LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U2(nolock) ON l.LastChangedUserId = U2.Id LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U3(nolock) ON l.LastChangedObjectsUserId = U3.Id
                         LEFT JOIN dbo.PaymentMergePurchaseView AS pay_p(nolock) ON pay_p.PurchaseId = p.Id
                         LEFT JOIN dbo.PaymentMergeContractView AS pay_c(nolock) ON pay_c.ContractId = c.Id";
            if (level == true)
            {
                query = @"SELECT   top 50000     p.Id AS PurchaseId, p.Number AS PurchaseNumber, l.Id AS LotId, l.Number AS LotNumber, l.Sum as LotSum, p.Name AS PurchaseName, p.DateBegin, p.DateEnd, 
lt.Name AS LawTypeName, nat.Name AS NatureName, cat.Name AS CategoryName,
                          l.SourceOfFinancing, lf.FundingNames, p.DeliveryTime, dtp.DeliveryTimePeriod_text, p.URL, ls.Name AS LotStatusName, 
customer.FullName AS CustomerFullName, customer.ShortName AS CustomerShortName, 
                         customerRegion.FederationSubject AS CustomerFederationSubject, customerOrganizationType.Name AS CustomerOrganizationType, customer.INN AS CustomerINN, customer.OutId AS CustomerId, 
                         receiver.FullName AS ReceiverFullName, receiver.ShortName AS ReceiverShortName, receiverRegion.FederationSubject AS ReceiverFederationSubject, receiverOrganizationType.Name AS ReceiverOrganizationType, 
                         receiver.INN AS ReceiverINN, receiver.OutId AS ReceiverId, 'слишком долго ждать' AS CustomerSource, '?' KBKs, '?' contract_KBKs, pc.Name AS PurchaseClassName, U1.UserName AS LastChangedUser_Purchase,
                          U2.UserName AS LastChangedUser_Lot, U3.UserName AS LastChangedUser_PurchaseObjectReady, c.Id AS contractId, por.Id AS porID, p.Comment, nat2.Name AS Nature_L2Name
FROM            dbo.Purchase AS p(nolock) inner join
dbo.Lot AS l(nolock) ON l.PurchaseId = p.Id
LEFT JOIN dbo.Contract AS c(nolock) ON c.LotId = l.Id
LEFT JOIN dbo.PurchaseObjectReady AS por(nolock) ON por.LotId = l.Id
LEFT JOIN dbo.Nature_L2 AS nat2(nolock) ON p.Nature_L2Id = nat2.Id LEFT JOIN
                         dbo.Nature AS nat(nolock) ON nat.Id = p.NatureId LEFT JOIN
                         dbo.LawType AS lt(nolock) ON p.LawTypeId = lt.Id LEFT JOIN
                         dbo.PurchaseClass AS pc(nolock) ON pc.Id = p.PurchaseClassId LEFT JOIN
                         dbo.OrganizationOut AS customer(nolock) ON customer.Id = p.CustomerId LEFT JOIN
                         dbo.OrganizationType AS customerOrganizationType(nolock) ON customerOrganizationType.Id = customer.OrganizationTypeId LEFT JOIN
                         dbo.Region AS customerRegion(nolock) ON customerRegion.Id = customer.RegionId LEFT JOIN
                         dbo.Category AS cat(nolock) ON nat.CategoryId = cat.Id LEFT JOIN
                         dbo.OrganizationOut AS receiver(nolock) ON receiver.Id = por.ReceiverId LEFT JOIN
                         dbo.OrganizationType AS receiverOrganizationType(nolock) ON receiverOrganizationType.Id = receiver.OrganizationTypeId LEFT JOIN
                         dbo.Region AS receiverRegion(nolock) ON receiverRegion.Id = receiver.RegionId LEFT JOIN
                         dbo.LotFundingMergeLotView(nolock) AS lf ON lf.LotId = l.Id  LEFT JOIN
                         dbo.SupplierResult AS sr(nolock) ON sr.LotId = l.Id LEFT JOIN
                         dbo.LotStatus AS ls(nolock) ON ls.Id = sr.LotStatusId LEFT JOIN
                         dbo.DeliveryTimeInfoMergePurchaseView AS dtp(nolock) ON dtp.PurchaseId = p.Id   LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U1(nolock) ON p.LastChangedUserId = U1.Id LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U2(nolock) ON l.LastChangedUserId = U2.Id LEFT JOIN
                         DataAggregator.dbo.AspNetUsers AS U3(nolock) ON l.LastChangedObjectsUserId = U3.Id";
            }
            query+= @" 
" + filter.Where_Standard;
            var viewData = _context.Database.SqlQuery<DataAggregator.Domain.Model.GovernmentPurchases.View.MassFixesDataView>(query);
            _context.Database.CommandTimeout = 0;
            result.Add("data", viewData);
            result.Add("count", viewData.Count());

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult InitMiniClassifiers()
        {
            var empty = new dictspr(){ Id = 0, Value = "без изменений" };

            JsonResult ret = new JsonResult();
            var Nature_1 =
                _context.Nature
                    .OrderBy(o => o.Name)
                    .Select(s => new dictspr() { Id = s.Id,  Value = s.Name })
                    .ToList();
            Nature_1.Insert(0, empty);
            ret.Nature = Nature_1;
            var Nature_2 =
                _context.Nature_L2
                    .OrderBy(o => o.Nature_L1.Name).OrderBy(o2=> o2.Name)
                    .Select(s => new dictspr() { Id = s.Id, Value = s.Name  +" \\ "+s.Nature_L1.Name })
                    .ToList();
            Nature_2.Insert(0, empty);
            ret.Nature_L2 = Nature_2;

            ret.deliveryTimePeriodList = _context.DeliveryTimePeriod.Select(s => new { Id = s.Id, Name = s.Name }).ToList();

            //ret.Funding = _context.Funding.OrderBy(o=>o.Code).Select(s => new { Id = s.Id, Name =s.Code+" "+ s.Name, Value=false }).ToList();

            //SourceOfFinancing = lot.SourceOfFinancing;

            var dbFundings = _context.Funding.OrderBy(f => f.Code).ToList();
            ret.FundingList = dbFundings.Select(f => new DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases.FundingJson(f, null)).ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
            };
            return jsonNetResult;
        }
        /*
        [HttpPost]
        public ActionResult searchReceiver(string value, int? count)
        {
            List<dictspr> ret = new List<dictspr>();
            ret.Add(new dictspr() { Id = 0, Value = "без изменений" });
            --Получатель (выбор из справочника по id, ИНН, Наименованию (полному и сокращенному), Адресу (LocationAddress), Региону, типу учреждения)
            var r_fullName = _context.Organization.Select(s => s);
            string where = "";
            foreach (string s_1 in value.Split(' '))
            {
                if (!string.IsNullOrEmpty(s_1))
                {
                    long l_test = 0;
                    if (long.TryParse(s_1, out l_test) == true)
                    {
                        var r1 = _context.Organization.Where(w => w.Id == l_test || w.INN.Contains(s_1)).Select(s => new dictspr() { Id = s.Id, Value = s.FullName+" "+s.INN+" "+s.LocationAddress }).Take(100);
                        ret.AddRange(r1);
                    }
                    if (s_1.Length> 2)
                    {
                        r_fullName = r_fullName.Where(w => w.FullName.Contains(s_1) || w.LocationAddress.Contains(s_1));
                        if (where != "") where += " and ";
                        where += " value like '%" + s_1 + "%'";
                    }
                }
            }
            if (!string.IsNullOrEmpty(where))
            {
                //var r_fullName = _context.Database.SqlQuery<dictspr>("select ID ,Value from [OrganizationDict] where " + where);

                //ret.AddRange(r_fullName);
                ret.AddRange(r_fullName.Select(s => new dictspr() { Id = s.Id, Value = s.FullName + " " + s.INN + " " + s.LocationAddress }).Take(100));
            }
            return new JsonNetResult(ret);
        }
*/
        [HttpPost]
        public ActionResult setchange(ChangeClass change)
        {

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = "ok"
            };

            //обновление Purchase
            if (change.PurchaseIds != null)
            {
                foreach (var PurchaseID in change.PurchaseIds)
                {
                    //NatureId
                    //DeliveryTimeInfo     
                    if (!string.IsNullOrEmpty(change.Comment)&&User.IsInRole("GManager"))
                    {
                        var update_Purchase_comment = _context.Purchase.Where(w => w.Id == PurchaseID).FirstOrDefault();
                        update_Purchase_comment.Comment = change.Comment;
                        
                    }
                    if (change.NatureId > 0 && User.IsInRole("GOperator"))
                    {
                        var nature = _context.Nature.Where(w => w.Id == change.NatureId).FirstOrDefault();
                        var update_Purchase = _context.Purchase.Where(w => w.Id == PurchaseID).FirstOrDefault();

                        update_Purchase.NatureId = nature.Id;
                        update_Purchase.CategoryId = nature.CategoryId;
                        update_Purchase.Nature_L2Id = null;

                        update_Purchase.LastChangedDate = DateTime.Now;
                        update_Purchase.LastChangedUserId = new Guid(User.Identity.GetUserId());
                        
                    }
                    if (change.Nature_L2Id > 0 && User.IsInRole("GManager"))
                    {
                        var nature_L2 = _context.Nature_L2.Where(w => w.Id == change.Nature_L2Id).FirstOrDefault();
                        var nature_L1 = _context.Nature.Where(w => w.Id == nature_L2.Nature_L1Id).FirstOrDefault();
                        var update_Purchase = _context.Purchase.Where(w => w.Id == PurchaseID).FirstOrDefault();

                        update_Purchase.Nature_L2Id = nature_L2.Id;
                        update_Purchase.NatureId = nature_L1.Id;
                        update_Purchase.CategoryId = nature_L1.CategoryId;

                        update_Purchase.LastChangedDate = DateTime.Now;
                        update_Purchase.LastChangedUserId = new Guid(User.Identity.GetUserId());
                        
                    }
                    if (change.DeliveryTimeInfo != null && User.IsInRole("GManager"))
                    {
                        var update_Purchasedt = _context.Purchase.Where(w => w.Id == PurchaseID).FirstOrDefault();
                        var dti_del = _context.DeliveryTimeInfo.Where(w => w.PurchaseId == PurchaseID);
                        _context.DeliveryTimeInfo.RemoveRange(dti_del);
                        _context.SaveChanges();
                        foreach (var DTI in change.DeliveryTimeInfo)
                        {
                            _context.DeliveryTimeInfo.Add(new Domain.Model.GovernmentPurchases.DeliveryTimeInfo()
                            {
                                PurchaseId = PurchaseID,
                                DateStart = DTI.DateStart,
                                DateEnd = DTI.DateEnd,
                                Count = DTI.Count,
                                DeliveryTimePeriodId = DTI.DeliveryTimePeriod.Id
                            });
                        }
                        update_Purchasedt.LastChangedDate = DateTime.Now;
                        update_Purchasedt.LastChangedUserId = new Guid(User.Identity.GetUserId());
                        
                    }
                }
                _context.SaveChanges();
            }
            //обновление Lot
            if (change.LotIds != null && User.IsInRole("GOperator"))
            {
                foreach (var LotID in change.LotIds)
                {                    
                    //FundingIds
                    if (change.FundingIds != null)
                    {
                        var update_lot = _context.Lot.Where(w => w.Id == LotID).FirstOrDefault();
                        var lf_del = _context.LotFunding.Where(w => w.LotId == LotID);
                        _context.LotFunding.RemoveRange(lf_del);
                        _context.SaveChanges();
                        foreach (var FundingId in change.FundingIds)
                        {
                            _context.LotFunding.Add(new Domain.Model.GovernmentPurchases.LotFunding()
                            {
                                LotId = LotID,
                                FundingId = FundingId
                            });
                        }
                        update_lot.LastChangedDate = DateTime.Now;
                        update_lot.LastChangedUserId = new Guid(User.Identity.GetUserId());
                        
                    }
                }
                _context.SaveChanges();
            }
            //обновление Purchase
            if (change.PurchaseObjectReadyIds != null && User.IsInRole("GManager"))
            {
                foreach (var PurchaseObjectReadyID in change.PurchaseObjectReadyIds)
                {
                    //ReceiverId
                    if (change.ReceiverId > 0)
                    {
                        var update_POR = _context.PurchaseObjectReady.Where(w => w.Id == PurchaseObjectReadyID).FirstOrDefault();
                        update_POR.ReceiverId = change.ReceiverId;
                        //update_POR.ReceiverRaw=change.re
                        update_POR.Lot.LastChangedObjectsDate = DateTime.Now;
                        update_POR.Lot.LastChangedObjectsUserId = new Guid(User.Identity.GetUserId());
                       
                    }
                }
                _context.SaveChanges();
            }
            return jsonNetResult;
        }
        public class dictch
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public bool Value { get; set; }
        }
        public class dictspr
        {
            public long Id { get; set; }
            public string Value { get; set; }
        }
        public class JsonResult
        {
            public object Nature { get; set; }
            public object Nature_L2 { get; set; }
            public object FundingList { get; set; }
            public object deliveryTimePeriodList { get; set; }
        }
        public class ChangeClass
        {
            public Byte NatureId { get; set; }
            public Int16 Nature_L2Id { get; set; }
            public long ReceiverId { get; set; }
            public string Comment { get; set; }
            public List<DataAggregator.Domain.Model.GovernmentPurchases.DeliveryTimeInfo> DeliveryTimeInfo { get; set; }
            public List<Byte> FundingIds { get; set; }

            public List<long> PurchaseIds { get; set; }
            public List<long> LotIds { get; set; }
            public List<long> PurchaseObjectReadyIds { get; set; }
        }
    }
}