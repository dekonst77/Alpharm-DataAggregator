using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PurchasesFilterJson
    {
        public string Id { get; set; }

        public string Number { get; set; }
        public string ReestrNumber { get; set; }

        public Guid? LastChangedPurchaseUser { get; set; }

        public Guid? LastChangedLotUser { get; set; }

        public Guid? LastChangedObjectReadyUser { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Name { get; set; }

        public Byte? NatureId { get; set; }

        public Byte? FundingId { get; set; }

        public Byte? PurchaseClassId { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string FederalDistrict { get; set; }

        public string FederationSubject { get; set; }

        public string GetFilter(int count, Guid userGuid)
        {
            string whereString = "";

            if (!string.IsNullOrEmpty(Id))
            {
                whereString += " and p.Id in (" +string.Join(",", DataAggregator.Web.Controllers.BaseController.GetListLong(Id)) + ")";
            }

            if (!string.IsNullOrEmpty(ReestrNumber))
            {
                whereString += @" and p.id in (
SELECT        SL.PurchaseId
FROM            dbo.Contract SC INNER JOIN
                         dbo.Lot SL ON SC.LotId = SL.Id
where [ReestrNumber]in(" + string.Join(",", DataAggregator.Web.Controllers.BaseController.GetListString(ReestrNumber)) + "))";
            }

            if (!string.IsNullOrEmpty(Number))
            {
                whereString += " and p.Number in (" + string.Join(",", DataAggregator.Web.Controllers.BaseController.GetListString(Number)) + ")";
            }

            if (LastChangedPurchaseUser != null)
            {
                whereString += " and p.LastChangedUserId = '" + LastChangedPurchaseUser + "'";
            }

            if (LastChangedLotUser != null)
            {
                whereString += " and l.LastChangedUserId = '" + LastChangedLotUser + "'";
            }

            if (LastChangedObjectReadyUser != null)
            {
                whereString += " and l.LastChangedObjectsUserId = '" + LastChangedObjectReadyUser + "'";
            }

            if (DateStart != null)
            {
                whereString += " and DATEDIFF(DAY,'" + ((DateTime)DateStart).ToShortDateString() + "', DateBegin) >= 0";
            }

            if (DateEnd != null)
            {
                whereString += " and DATEDIFF(DAY,'" + ((DateTime)DateEnd).ToShortDateString() + "', DateBegin) <= 0";
            }

            if (!string.IsNullOrEmpty(Name))
            {
                whereString += " and p.Name like '%" + Name + "%'";
            }

            //Характер
            if (NatureId != null)
            {
                if (NatureId == 0)
                {
                    //Не задан
                    whereString += " and p.NatureId is null ";
                }
                else
                {
                    whereString += " and p.NatureId = " + NatureId;
                }
                
            }

            //Финансирование
            if (FundingId != null)
            {
                if (FundingId == 0)
                {
                    //Не задан
                    whereString += " and lf.FundingId is null";
                }
                else
                {
                    whereString += " and lf.FundingId = " + FundingId;
                }
                
            }

            if (PurchaseClassId != null)
            {
                whereString += " and p.PurchaseClassId = " + PurchaseClassId;
            }

            if (!string.IsNullOrEmpty(City))
            {
                whereString += " and r.City like '%" + City + "%'";
            }

            if (!string.IsNullOrEmpty(District))
            {
                whereString += " and r.District like '%" + District + "%'";
            }

            if (!string.IsNullOrEmpty(FederalDistrict))
            {
                whereString += " and r.FederalDistrict like '%" + FederalDistrict + "%'";
            }

            if (!string.IsNullOrEmpty(FederationSubject))
            {
                whereString += " and r.FederationSubject like '%" + FederationSubject + "%'";
            }

            var regionJoinNeeded = !string.IsNullOrEmpty(City) ||
                                   !string.IsNullOrEmpty(District) ||
                                   !string.IsNullOrEmpty(FederalDistrict) ||
                                   !string.IsNullOrEmpty(FederationSubject);

            var regionJoin = regionJoinNeeded ?
                " left outer join dbo.Organization o on o.Id = p.CustomerId left outer join Region r on o.RegionId = r.Id " :
                "";

            var sql = @"select top(" + count + @") p.Id
	                    from dbo.Purchase as p
	                    inner join dbo.StatusHistory as sh on sh.PurchaseId = p.Id
                        left outer join dbo.Lot as l on l.PurchaseId = p.Id
                        left outer join dbo.LotFunding as lf on lf.LotId = l.Id " + regionJoin +
	                    @"where sh.StatusId in (100, 1000) and sh.IsActual = 1 " +
                        whereString + @"
                        group by p.Id, p.AssignedToUserId, p.HigherPriority, p.DateBegin
	                    order by p.AssignedToUserId desc, p.HigherPriority desc, p.DateBegin desc";

            return sql;
        }
    }
}