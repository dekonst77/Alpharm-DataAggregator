using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class OrganizationJson
    {
        public OrganizationJson()
        {
            
        }

        public OrganizationJson(GovernmentPurchasesContext context,Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException("organization");
            }
            Id = organization.Id;
            if (organization.ActualId > 0)
            {
                organization = context.Organization.Where(w => w.Id == organization.ActualId).Single();
            }            
            OrganizationType = organization.OrganizationType == null
                ? new DictionaryElementJson() { Id = null, Name = null }
                : new DictionaryElementJson()
                {
                    Id = organization.OrganizationType.Id,
                    Name = organization.OrganizationType.Name
                };
            INN = organization.INN;
            FullName = organization.FullName;
            ShortName = organization.ShortName;
            Address = organization.LocationAddress;
            KPP = organization.KPP;
            Region = organization.Region == null
                ? null
                : string.Format("{0} {1}", organization.Region.FederationSubject, organization.Region.District);
        }

        public long? Id { get; set; }

        public DictionaryElementJson OrganizationType { get; set; }

        public string INN { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public string Region { get; set; }

        public string Address { get; set; }

        public string KPP { get; set; }

       
    }
}