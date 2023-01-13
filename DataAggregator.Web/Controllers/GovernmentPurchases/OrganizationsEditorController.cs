using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DataAggregator.Web.Controllers.GS;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class OrganizationsEditorController : BaseController
    {
        [HttpPost]
        public ActionResult GetOrganizations(OrganizationsEditorFilterJson filter)
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                try
                {
                    var filterString = GetFilterString(filter);

                    var organizations = context.GetOrganizationsByFilter(filterString);

                    return new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = organizations
                    };

                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        [HttpPost]
        public ActionResult GetOrganizationsNew()
        {
            var N1 = new Domain.Model.GovernmentPurchases.View.OrganizationView();
            N1.Id = 0;
            N1.FZ = 0;
            N1.Is_CP = false;
            N1.Is_LO = false;
            N1.Is_Customer = true;
            N1.Is_Recipient = true;
            N1.comment = "";

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = N1
            };
        }

        [HttpPost]
        public ActionResult GetNature()
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                try
                {
                    
                    var result =context.Nature.OrderBy(ot => ot.Name).ToList();
                    ViewData["Nature"] = result;
                    ViewData["Category"] = context.Category.OrderBy(ot => ot.Name).ToList();
                    return new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = ViewData
                    };
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        [HttpPost]
        public ActionResult GetOrganizationTypes()
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                try
                {
                    var result =
                        context.OrganizationType.OrderBy(ot => ot.Name).ToList();

                    return new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = result
                    };
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        [HttpPost]
        public ActionResult GetRegionNames(int level)
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                try
                {
                    var result =
                        context.RegionName.Where(rn => rn.Level == level)
                            .OrderBy(rn => rn.Name)
                            .Select(rn => new { Id = rn.Id, displayValue = rn.Name })
                            .ToList();

                    return new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = result
                    };
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        [HttpPost]
        public ActionResult GetRegionList()
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                try
                {
                    var result =
                        context.Region.Where(w=>w.Level==4).OrderBy(r => r.FederalDistrict)
                            .ThenBy(r => r.FederationSubject)
                            .ThenBy(r => r.District)
                            .ThenBy(r => r.City)
                            .Select(
                                r =>
                                    new
                                    {
                                        Id = r.Id,
                                        FederalDistrict = r.FederalDistrict,
                                        FederationSubject = r.FederationSubject,
                                        District = r.District,
                                        City = r.City,
                                        Code = r.Code
                                    })
                            .ToList();

                    return new JsonNetResult
                    {
                        Formatting = Formatting.Indented,
                        Data = result
                    };
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult Organization_save(System.Collections.Generic.List<Domain.Model.GovernmentPurchases.View.OrganizationView> array)
        {
            using (var context = new GovernmentPurchasesContext(APP))
            {
                foreach (var UPD in array)
                {
                    if (UPD.Id == 0)
                    {
                        Domain.Model.GovernmentPurchases.Organization NEW =new  Domain.Model.GovernmentPurchases.Organization();
                        NEW.comment = "";
                        NEW.Is_Customer = false;
                        NEW.Is_Recipient = false;
                        NEW.Is_LO = false;
                        NEW.Is_CP = false;
                        context.Organization.Add(NEW);
                        context.SaveChanges();
                        UPD.Id = NEW.Id;
                    }
                    if (UPD.FixedNatureId == 0)
                        UPD.FixedNatureId = null;
                    var organization = context.Organization.FirstOrDefault(o => o.Id == UPD.Id);
                    organization.FullName = UPD.FullName;
                    organization.ShortName = UPD.ShortName;
                    organization.LocationAddress = UPD.LocationAddress;
                    organization.PostAddress = UPD.PostAddress;
                    organization.INN = UPD.INN;
                    organization.OGRN = UPD.OGRN;
                    organization.KPP = UPD.KPP;

                    organization.FixedNatureId = UPD.FixedNatureId;

                    organization.comment = UPD.comment == null?"" : UPD.comment;
                    organization.Is_CP = UPD.Is_CP;
                    organization.Is_LO = UPD.Is_LO;
                    organization.Is_Customer = UPD.Is_Customer;
                    organization.Is_Recipient = UPD.Is_Recipient;

                    organization.Url = UPD.Url;

                    organization.ActualId = UPD.ActualId;

                    organization.OrganizationTypeId = UPD.OrganizationTypeId;
                    organization.RegionId = UPD.RegionId;
                    organization.RegionOfLocalizationId = UPD.RegionOfLocalizationId;

                    if (organization.ActualId == organization.Id)
                        organization.ActualId = null;

                    organization.LastChangedUserId = new Guid(User.Identity.GetUserId());
                    organization.LastChangedDate = DateTime.Now;
                    if (organization.GosZakId != UPD.GosZakId)
                    {
                        organization.GosZakId = UPD.GosZakId;
                        if (organization.GosZakId > 0)
                        {
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("exec [GovernmentPurchasesLoader].[search].[OrganizationParse] @OrganizationURL,@OrganizationID,0",
                                new System.Data.SqlClient.SqlParameter("@OrganizationURL", organization.Url),
                                new System.Data.SqlClient.SqlParameter("@OrganizationID", organization.GosZakId));
                                }
                    }
                }
                context.SaveChanges();
                context.Organization_Refresh();
            }
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };
            return jsonNetResult;
        }
        private string GetFilterString(OrganizationsEditorFilterJson filter)
        {
            var result = new StringBuilder();
            if (filter.OnlyDrugsLinked)
            {
                result.Append(
                    @"inner join
(select p.CustomerId as OrganizationId from dbo.Purchase as p(nolock) where p.PurchaseClassId = 2 
union 
select contr.ReceiverId as OrganizationId from dbo.Contract as contr(nolock) inner join dbo.Lot as cLot(nolock) on cLot.Id = contr.LotId 
inner join dbo.Purchase as cPur(nolock) on cPur.Id = cLot.PurchaseId where cPur.PurchaseClassId = 2 
union 
select por.ReceiverId as OrganizationId from PurchaseObjectReady as por(nolock) inner join dbo.Lot as pLot(nolock) on pLot.Id = por.LotId 
inner join dbo.Purchase as pPur(nolock) on pPur.Id = pLot.PurchaseId where pPur.PurchaseClassId = 2 
) as t 
inner join [dbo].[OrganizationOut] OO on t.OrganizationId=OO.Id
on OO.[OutId] = o.Id "
                    );
            }
            result.Append("WHERE 1=1 ");

            if (filter.is_Actual)
                result.Append("AND o.ActualId is null");
            if (filter.is_CP)
                result.Append("AND o.is_CP=1");
            if (filter.is_LO)
                result.Append("AND o.is_LO=1");
            if (filter.no_iin)
                result.Append("AND o.inn is null");

            if (!string.IsNullOrEmpty(filter.Inn))
            {
                filter.Inn=string.Join("','", filter.Inn.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                result.Append(string.Format(" AND o.INN IN ('{0}')", filter.Inn));
            }

            if (!string.IsNullOrEmpty(filter.Id))
            {
                filter.Id = filter.Id.Replace(' ',',');
                result.Append(string.Format(" AND o.Id in ({0})", filter.Id));
            }

            if (filter.OrganizationType != null)
            {
                result.Append(string.Format(" AND o.OrganizationTypeId = {0}", filter.OrganizationType.Id));
            }

            if (filter.OnlyEmptyType)
            {
                result.Append(" AND o.OrganizationTypeId is null");
            }

            if (!string.IsNullOrEmpty(filter.ShortName))
            {
                result.Append(string.Format(" AND o.ShortName LIKE '%{0}%'", filter.ShortName));
            }

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                result.Append(string.Format(" AND o.FullName LIKE '%{0}%'", filter.FullName));
            }
            if (!string.IsNullOrEmpty(filter.Text))
            {
                result.Append(string.Format(" AND (o.FullName LIKE '%{0}%' or o.[LocationAddress] LIKE '%{0}%')", filter.Text));
            }

            if (filter.SelectedFederalDistrictNames != null)
            {
                var tempStrB = new StringBuilder();
                foreach (var fdName in filter.SelectedFederalDistrictNames)
                {
                    tempStrB.Append((tempStrB.Length > 0 ? " OR " : "") +
                                    "o.FederalDistrict LIKE '" + fdName + "'");
                }

                result.Append(" AND (" + tempStrB + ')');
            }

            if (filter.SelectedFederationSubjectNames != null)
            {
                var tempStrB = new StringBuilder();
                foreach (var fsName in filter.SelectedFederationSubjectNames)
                {
                    tempStrB.Append((tempStrB.Length > 0 ? " OR " : "") +
                                    "o.FederationSubject LIKE '" + fsName + "'");
                }

                result.Append(" AND (" + tempStrB + ')');
            }

            if (filter.OnlyEmptyRegion)
            {
                result.Append(" AND o.RegionId is null");
            }

            //if (filter.OnlyDrugsLinked)
            //{
            //    result.Append(
            //        " AND (o.Id IN (SELECT p.CustomerId FROM Purchase AS p with(nolock)) or " +
            //              "o.Id IN (SELECT c.ReceiverId FROM Contract AS c with(nolock)) or " +
            //              "o.Id IN (SELECT por.ReceiverId FROM PurchaseObjectReady AS por with(nolock)))"
            //        );
            //}

            return result.ToString();
        }
    }
}