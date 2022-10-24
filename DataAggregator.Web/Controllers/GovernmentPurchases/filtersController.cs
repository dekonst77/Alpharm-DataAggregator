using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.MassFixesData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DataAggregator.Domain.Model.GovernmentPurchases;
namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    public class filtersController : BaseController
    {
        public class filterWhere
        {
            public string Where_Purchase { get; set; }
            public string Where_Lot { get; set; }
            public string Where_PurchaseObjectReady { get; set; }

            public string Where_Standard { get; set; }


            //public string Where_Mini_Purchase { get; set; }
            //public string Where_Mini_Purchase { get; set; }
            //public string Where_Mini_Lot { get; set; }
            //public string Where_Mini_PurchaseObjectReady { get; set; }


            public string Where(string Purchase_ID, string Lot_ID, string PurchaseObjectReady_ID)
            {
                string ret = "";
                if (!string.IsNullOrEmpty(Where_Purchase) && Purchase_ID!="")
                {
                    if (ret != "") ret += " and ";
                    ret += "(" + Purchase_ID + " in (select [Id] from [dbo].[Purchase] filter_P(nolock) where " + Where_Purchase + "))";
                }
                if (!string.IsNullOrEmpty(Where_Lot) && Lot_ID!="")
                {
                    if (ret != "") ret += " and ";
                    ret += "(" + Lot_ID + " in (select [Id] from [dbo].[Lot] filter_L(nolock) where " + Where_Lot + "))";
                }
                if (!string.IsNullOrEmpty(Where_PurchaseObjectReady) && PurchaseObjectReady_ID!="")
                {
                    if (ret != "") ret += " and ";
                    ret += "(" + PurchaseObjectReady_ID + " in (select [Id] from [dbo].[PurchaseObjectReady] filter_POR(nolock) where " + Where_PurchaseObjectReady + "))";
                }
                if (ret != "")
                    ret = " Where " + ret;
                return ret;
            }
        }
        public class filterMain
        {
            public string Purchase_Id { get; set; }
            public PurchaseClass Purchase_PurchaseClass { get; set; }
            public string Purchase_Number { get; set; }
            public string Purchase_Customer_FederationSubject { get; set; }
            public string Purchase_Customer_FederalDistrict { get; set; }
            public string Lot_Id { get; set; }
            public string Purchase_Customer_name { get; set; }
            public string Purchase_DateBegin_start { get; set; }
            public string Purchase_DateBegin_end { get; set; }
            public string Purchase_Customer_INN { get; set; }
            public string Purchase_DateEnd_start { get; set; }
            public string Purchase_DateEnd_end { get; set; }
            public OrganizationType Purchase_Customer_OrganizationType { get; set; }
            public string Purchase_Name { get; set; }
            public string Purchase_CustomerId { get; set; }
            public Category Purchase_Category { get; set; }
            public string PurchaseObjectReady_Receiver_FederationSubject { get; set; }
            public Nature Purchase_Nature { get; set; }
            public Nature_L2 Purchase_Nature_L2 { get; set; }
            public string PurchaseObjectReady_Receiver_Name { get; set; }
            public Funding LotFunding_Funding { get; set; }
            public string PurchaseObjectReady_Receiver_INN { get; set; }
            public DeliveryTimePeriod DeliveryTimeInfo_DeliveryTimePeriod { get; set; }
            public OrganizationType PurchaseObjectReady_Receiver_OrganizationType { get; set; }
            public string Payment_KBK { get; set; }
            public string PurchaseObjectReady_ReceiverId { get; set; }
            public LotStatus SupplierResult_LotStatus { get; set; }

            public bool isPurchaseObjectReady { get; set; }

            public bool Not_Is_Recipient { get; set; }

            public string TriggerLog_What { get; set; }
            public string TriggerLog_When_start { get; set; }
            public string TriggerLog_When_end { get; set; }

        }
        public class JsonResult
        {
            public object Category { get; set; }
            public object Nature { get; set; }
            public object Nature_L2 { get; set; }
            public object Funding { get; set; }
            public object FederationSubject { get; set; }
            public object FederalDistrict { get; set; }
            public object PurchaseClass { get; set; }
            public object OrganizationType { get; set; }
            public object DeliveryTimePeriod { get; set; }
            public object LotStatus { get; set; }
        }
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~filtersController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult InitClassifiers()
        {
            JsonResult ret = new JsonResult();

            ret.Category = _context.Category.OrderBy(n => n.Name).ToList();
            ret.FederationSubject = _context.Region.Select(rn => rn.FederationSubject).Distinct().OrderBy(fs => fs).ToList();
            ret.FederalDistrict = _context.Region.Select(rn => rn.FederalDistrict).Distinct().OrderBy(fs => fs).ToList();
            var Nature=_context.Nature.OrderBy(n => n.Name).ToList();
            Nature.Insert(0, new Domain.Model.GovernmentPurchases.Nature() { Id = 0, Name = "пусто" });
            ret.Nature = Nature;

            var Nature_L2 = _context.Nature_L2.OrderBy(n => n.Name).ToList();
            Nature_L2.Insert(0, new Domain.Model.GovernmentPurchases.Nature_L2() { Id = 0, Name = "пусто" });
            ret.Nature_L2 = Nature_L2;

            var Funding = _context.Funding.OrderBy(n => n.Name).ToList();
            Funding.Insert(0, new Domain.Model.GovernmentPurchases.Funding() { Id = 0, Name = "пусто" });
            ret.Funding = Funding;
            ret.PurchaseClass = _context.PurchaseClass.OrderBy(n => n.Name).ToList();
            ret.OrganizationType = _context.OrganizationType.OrderBy(n => n.Name).ToList();
            var DeliveryTimePeriod = _context.DeliveryTimePeriod.OrderBy(n => n.Name).ToList();
            DeliveryTimePeriod.Insert(0, new Domain.Model.GovernmentPurchases.DeliveryTimePeriod() { Id = 0, Name = "пусто" });
            ret.DeliveryTimePeriod = DeliveryTimePeriod;
            ret.LotStatus = _context.LotStatus.OrderBy(n => n.Name).ToList();

            
             JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data =  ret
            };
            return jsonNetResult;
        }

        string Getlist(string s1,bool isText,bool isIn,string fieldname)
        {
            string ret="";
            if (isIn)
            {
                foreach (string s in s1.Split(','))
                {
                    string s2 = s.Trim();
                    if (!string.IsNullOrEmpty(s2))
                    {
                        s2 = s2.Replace("'", "''");
                        if (isText)
                            s2 = "'" + s2 + "'";
                        if (ret != "")
                            ret += ",";
                        ret += s2;
                    }
                }
                if (ret != "")
                {
                    ret = "" + fieldname + " in (" + ret + ")";
                }
            }
            else
            {
                foreach (string s in s1.Split(','))
                {
                    string s2 = s.Trim();
                    if (!string.IsNullOrEmpty(s2))
                    {
                        s2 = s2.Replace("'", "''");
                        if (ret != "")
                            ret += " OR ";
                        if (s2 == "NULL")
                        {
                            ret += "" + fieldname + " is NULL ";
                        }
                        else
                        {
                            if (isText)
                                s2 = "'%" + s2 + "%'";
                            ret += "" + fieldname + " like " + s2;
                        }
                    }
                }

            }
            return ret;
        }
        string AddFilter(string s_in, string s_add)
        {
            if (s_in != "") s_in += " and ";
            s_in += s_add;
            return s_in;
        }

        [HttpPost]
        public ActionResult createFilter(filterMain filter)
        {
            filterWhere ret = new filterWhere();
            ret.Where_Purchase = "";
            ret.Where_Lot = "";
            ret.Where_PurchaseObjectReady = "";
            ret.Where_Standard = "";

            string Where_Customer = "";
            string Where_Receiver = "";

            string temp = "";
            if (!string.IsNullOrEmpty(filter.Purchase_Id))
            {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Id in (" + string.Join(",",GetListLong(filter.Purchase_Id)) + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Id in (" + string.Join(",", GetListLong(filter.Purchase_Id)) + "))");
            }
            if (filter.Purchase_PurchaseClass!=null)
            {
                if (filter.Purchase_PurchaseClass.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Nature_L2Id is null)");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Nature_L2Id is null)");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.PurchaseClassId = " + Convert.ToString(filter.Purchase_PurchaseClass.Id) + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.PurchaseClassId=" + Convert.ToString(filter.Purchase_PurchaseClass.Id) + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_Number))
            {
                ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Number in (" + string.Join(",", GetListString(filter.Purchase_Number)) + "))");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Number in (" + string.Join(",", GetListString(filter.Purchase_Number)) + "))");
            }
            if (!string.IsNullOrEmpty(filter.Purchase_Customer_FederationSubject))
            {
                temp = Getlist(filter.Purchase_Customer_FederationSubject, true, true, "FederationSubject");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Customer = AddFilter(Where_Customer, "(filter_Customer.RegionId IN (select ID from [dbo].Region where " + temp + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_Customer_FederationSubject, true, true, "customerRegion.FederationSubject") + ")");
                }
            }

            if (!string.IsNullOrEmpty(filter.Purchase_Customer_FederalDistrict))
            {
                temp = Getlist(filter.Purchase_Customer_FederalDistrict, true, true, "FederalDistrict");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Customer = AddFilter(Where_Customer, "(filter_Customer.RegionId IN (select ID from [dbo].Region where " + temp + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_Customer_FederalDistrict, true, true, "customerRegion.FederalDistrict") + ")");
                }
            }

            if (!string.IsNullOrEmpty(filter.Lot_Id))
            {
                temp = Getlist(filter.Lot_Id, false, true, "filter_L.Id");
                if (!string.IsNullOrEmpty(temp))
                {
                    ret.Where_Lot = AddFilter(ret.Where_Lot, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Lot_Id, false, true, "l.Id") + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_Customer_name))
            {
                temp = Getlist(filter.Purchase_Customer_name, true, false, "filter_Customer.FullName");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Customer = AddFilter(Where_Customer, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_Customer_name, true, false, "customer.FullName") + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.TriggerLog_What)&& !string.IsNullOrEmpty(filter.TriggerLog_When_start) && !string.IsNullOrEmpty(filter.TriggerLog_When_end))
            {
                //SELECT[Purchase_Id],[Who],[What],[When] FROM[GovernmentPurchases].[logs].[TriggerLog]
                DateTime TriggerLog_When_start_dt = DateTime.ParseExact(filter.TriggerLog_When_start.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string TriggerLog_When_start_st = TriggerLog_When_start_dt.ToString("yyyyMMdd");

                DateTime TriggerLog_When_end_dt = DateTime.ParseExact(filter.TriggerLog_When_end.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string TriggerLog_When_end_st = TriggerLog_When_end_dt.ToString("yyyyMMdd");

                ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Id in(SELECT[Purchase_Id] from [GovernmentPurchases].[logs].[TriggerLog](nolock) where [What] like '%"+ filter.TriggerLog_What + "%' AND [When] between '" + TriggerLog_When_start_st + "' and '" + TriggerLog_When_end_st + "'))");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Id in(SELECT[Purchase_Id] from [GovernmentPurchases].[logs].[TriggerLog](nolock) where [What] like '%" + filter.TriggerLog_What + "%' AND [When] between '" + TriggerLog_When_start_st + "' and '" + TriggerLog_When_end_st + "'))");
            }
                if (!string.IsNullOrEmpty(filter.Purchase_DateBegin_start))
            {
                //DateTime b1 = Convert.ToDateTime(filter.Purchase_DateBegin_start);
                DateTime b1 = DateTime.ParseExact(filter.Purchase_DateBegin_start.Substring(0,10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string ds1 = b1.ToString("yyyyMMdd");
                if (!string.IsNullOrEmpty(filter.Purchase_DateBegin_end))
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateBegin>='" + ds1 + "')");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateBegin>='" + ds1 + "')");
                }
                else
                {//Если нет ограничения сверху то типо точно эта дата
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateBegin='" + ds1 + "')");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateBegin='" + ds1 + "')");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_DateBegin_end))
            {
                DateTime b2 = DateTime.ParseExact(filter.Purchase_DateBegin_end.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string ds2 = b2.AddDays(1).ToString("yyyyMMdd");
                ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateBegin<'" + ds2 + "')");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateBegin<'" + ds2 + "')");
            }
            if (!string.IsNullOrEmpty(filter.Purchase_DateEnd_start))
            {
                DateTime e1 = DateTime.ParseExact(filter.Purchase_DateEnd_start.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string es1 = e1.ToString("yyyyMMdd");
                if (!string.IsNullOrEmpty(filter.Purchase_DateBegin_end))
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateEnd>='" + es1 + "')");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateEnd>='" + es1 + "')");
                }
                else
                {//Если нет ограничения сверху то типо точно эта дата
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateEnd='" + es1 + "')");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateEnd='" + es1 + "')");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_DateEnd_end))
            {
                DateTime e2 = DateTime.ParseExact(filter.Purchase_DateEnd_end.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                string es2 = e2.AddDays(1).ToString("yyyyMMdd");
                ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.DateEnd<'" + es2 + "')");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.DateEnd<'" + es2 + "')");

            }
            if (!string.IsNullOrEmpty(filter.Purchase_Customer_INN))
            {
                temp = Getlist(filter.Purchase_Customer_INN, true, true, "filter_Customer.INN");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Customer = AddFilter(Where_Customer, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_Customer_INN, true, true, "customer.INN") + ")");
                }
            }
            if (filter.Purchase_Customer_OrganizationType != null)
            {
                if (filter.Purchase_Customer_OrganizationType.Id == 0)
                {
                    Where_Customer = AddFilter(Where_Customer, "(filter_Customer.OrganizationTypeId is null)");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(customer.OrganizationTypeId is null)");
                }
                else
                {
                    Where_Customer = AddFilter(Where_Customer, "(filter_Customer.OrganizationTypeId = " + Convert.ToString(filter.Purchase_Customer_OrganizationType.Id) + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(customer.OrganizationTypeId=" + Convert.ToString(filter.Purchase_Customer_OrganizationType.Id) + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_Name))
            {
                temp = Getlist(filter.Purchase_Name, true, false, "filter_P.Name");
                if (!string.IsNullOrEmpty(temp))
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_Name, true, false, "p.Name") + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.Purchase_CustomerId))
            {
                temp = Getlist(filter.Purchase_CustomerId, false, true, "filter_Customer.Id");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Customer = AddFilter(Where_Customer, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.Purchase_CustomerId, false, true, "customer.OutId") + ")");
                }
            }
            if (filter.Purchase_Category!=null)
            {
                if (filter.Purchase_Category.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.CategoryId is null)");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.CategoryId is null)");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.CategoryId = " + Convert.ToString(filter.Purchase_Category.Id) + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.CategoryId=" + Convert.ToString(filter.Purchase_Category.Id) + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.PurchaseObjectReady_Receiver_FederationSubject))
            {
                temp = Getlist(filter.PurchaseObjectReady_Receiver_FederationSubject, true, true, "FederationSubject");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Receiver = AddFilter(Where_Receiver, "(filter_Receiver.RegionId IN (select ID from [dbo].Region where " + temp + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.PurchaseObjectReady_Receiver_FederationSubject, true, true, "receiverRegion.FederationSubject") + ")");
                }
            }
            if (filter.Purchase_Nature != null)
            {
                if (filter.Purchase_Nature.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.NatureId is null)");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.NatureId is null)");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.NatureId = " + Convert.ToString(filter.Purchase_Nature.Id) + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.NatureId=" + Convert.ToString(filter.Purchase_Nature.Id) + ")");
                }
            }
            if (filter.Purchase_Nature_L2 != null)
            {
                if (filter.Purchase_Nature_L2.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Nature_L2Id is null)");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Nature_L2Id is null)");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Nature_L2Id = " + Convert.ToString(filter.Purchase_Nature_L2.Id) + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Nature_L2Id=" + Convert.ToString(filter.Purchase_Nature_L2.Id) + ")");
                }
            }
            if (!string.IsNullOrEmpty(filter.PurchaseObjectReady_Receiver_Name))
            {
                temp = Getlist(filter.PurchaseObjectReady_Receiver_Name, true, false, "filter_Receiver.FullName");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Receiver = AddFilter(Where_Receiver, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.PurchaseObjectReady_Receiver_Name, true, false, "receiver.FullName") + ")");
                }
            }
            if (filter.LotFunding_Funding != null)
            {
                if (filter.LotFunding_Funding.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_L.Id not IN (select [LotId] from [dbo].[LotFunding]))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(l.Id not IN (select [LotId] from [dbo].[LotFunding]))");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_L.Id IN (select [LotId] from [dbo].[LotFunding](nolock) where [FundingId] = " + Convert.ToString(filter.LotFunding_Funding.Id) + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(l.Id IN (select [LotId] from [dbo].[LotFunding](nolock) where [FundingId]=" + Convert.ToString(filter.LotFunding_Funding.Id) + "))");
                }
            }
            if (!string.IsNullOrEmpty(filter.PurchaseObjectReady_Receiver_INN))
            {
                temp = Getlist(filter.PurchaseObjectReady_Receiver_INN, true, true, "filter_Receiver.INN");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Receiver = AddFilter(Where_Receiver, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.PurchaseObjectReady_Receiver_INN, true, true, "receiver.INN") + ")");
                }
            }
            if (filter.DeliveryTimeInfo_DeliveryTimePeriod != null)
            {
                if (filter.DeliveryTimeInfo_DeliveryTimePeriod.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Id NOT IN (select [PurchaseId] from [dbo].[DeliveryTimeInfo]))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Id NOT IN (select [PurchaseId] from [dbo].[DeliveryTimeInfo]))");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Id IN (select [PurchaseId] from [dbo].[DeliveryTimeInfo](nolock) where [DeliveryTimePeriodId] = " + Convert.ToString(filter.DeliveryTimeInfo_DeliveryTimePeriod.Id) + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Id IN (select [PurchaseId] from [dbo].[DeliveryTimeInfo](nolock) where [DeliveryTimePeriodId]=" + Convert.ToString(filter.DeliveryTimeInfo_DeliveryTimePeriod.Id) + "))");
                }
            }

            if (filter.Not_Is_Recipient)
            {
                Where_Receiver = AddFilter(Where_Receiver, "(filter_Receiver.Is_Recipient=0)");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(receiver.Is_Recipient=0)");
            }

            
            if (!string.IsNullOrEmpty(filter.Payment_KBK))
            {
                temp = Getlist(filter.Payment_KBK, true, true, "KBK");
                if (!string.IsNullOrEmpty(temp))
                {
                    //select [LotId] from [dbo].[LotFunding] where [FundingId] in (select [Id] from [dbo].[Funding] where [Name])
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.Id IN (select [PurchaseId] from [dbo].[Payment] where " + temp + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(p.Id IN (select [PurchaseId] from [dbo].[Payment] where " + temp + "))");
                }
            }

            if (!string.IsNullOrEmpty(filter.PurchaseObjectReady_ReceiverId))
            {
                temp = Getlist(filter.PurchaseObjectReady_ReceiverId, false, true, "filter_Receiver.Id");
                if (!string.IsNullOrEmpty(temp))
                {
                    Where_Receiver = AddFilter(Where_Receiver, "(" + temp + ")");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(" + Getlist(filter.PurchaseObjectReady_ReceiverId, false, true, "receiver.OutId") + ")");
                }
            }
            if (filter.SupplierResult_LotStatus != null)
            {
                if (filter.SupplierResult_LotStatus.Id == 0)
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_L.Id NOT IN (select [LotId] from [dbo].[SupplierResult](nolock)))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(l.Id NOT IN (select [LotId] from [dbo].[SupplierResult](nolock)))");
                }
                else
                {
                    ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_L.Id IN (select [LotId] from [dbo].[SupplierResult](nolock) where [LotStatusId] = " + Convert.ToString(filter.SupplierResult_LotStatus.Id) + "))");
                    ret.Where_Standard = AddFilter(ret.Where_Standard, "(l.Id IN (select [LotId] from [dbo].[SupplierResult](nolock) where [LotStatusId]=" + Convert.ToString(filter.SupplierResult_LotStatus.Id) + "))");
                }
            }


            if (filter.isPurchaseObjectReady)
            {
                //temp = Getlist(filter.isPurchaseObjectReady, false, true, "filter_Receiver.Id");
                //if (!string.IsNullOrEmpty(temp))
                //{
                ret.Where_Lot = AddFilter(ret.Where_Lot, "(filter_L.Id IN (select [LotId] from [dbo].[PurchaseObjectReady]))");
                ret.Where_Standard = AddFilter(ret.Where_Standard, "(por.id>0)");
                // }
            }

            if (!string.IsNullOrEmpty(Where_Customer))
            {
                Where_Customer = "select Id from Organization filter_Customer(nolock) where " + Where_Customer;
                ret.Where_Purchase = AddFilter(ret.Where_Purchase, "(filter_P.CustomerId IN (" + Where_Customer + "))");
            }
            if (!string.IsNullOrEmpty(Where_Receiver))
            {
                Where_Receiver = "select Id from Organization filter_Receiver(nolock) where " + Where_Receiver;
                ret.Where_PurchaseObjectReady = AddFilter(ret.Where_PurchaseObjectReady, "(filter_POR.ReceiverId IN (" + Where_Receiver + "))");
            }
            if (!string.IsNullOrEmpty(ret.Where_Standard))
            {
                ret.Where_Standard = " where " + ret.Where_Standard;
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ret
            };
            return jsonNetResult;
        }

    }
}