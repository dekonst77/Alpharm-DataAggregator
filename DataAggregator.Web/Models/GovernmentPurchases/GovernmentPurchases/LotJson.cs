using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class LotJson
    {
        public LotJson()
        {
            
        }

        public LotJson(GovernmentPurchasesContext context, Lot lot)
        {
            if (lot == null)
            {
                throw new ArgumentNullException("lot");
            }

            Id = lot.Id;
            Number = lot.Number;
            Sum = lot.Sum;
            SourceOfFinancing = lot.SourceOfFinancing;

            var dbFundings = context.Funding.OrderBy(f => f.Code).ToList();
            FundingList = dbFundings.Select(f => new FundingJson(f, lot)).ToList();

            var supplierResult = context.SupplierResult.SingleOrDefault(sr => sr.LotId == lot.Id);

            if (supplierResult != null)
                LotStatus = supplierResult.LotStatus.Name;

            LastChangedUser = lot.LastChangedUserId == null ? String.Empty : string.Format("{0}, {1:dd.MM.yyyy}", context.User.Single(u => u.Id == lot.LastChangedUserId.ToString()).FullNameWithoutPatronymic, lot.LastChangedDate);
            LastChangedUser_UserName = lot.LastChangedUserId == null ? String.Empty : string.Format("{0}", context.User.Single(u => u.Id == lot.LastChangedUserId.ToString()).UserName);
            LastChangedObjectsUser = lot.LastChangedObjectsUserId == null ? String.Empty : string.Format("{0}, {1:dd.MM.yyyy}", context.User.Single(u => u.Id == lot.LastChangedObjectsUserId.ToString()).FullNameWithoutPatronymic, lot.LastChangedObjectsDate);

            LastChangedObjectsUser_UserName = lot.LastChangedObjectsUserId == null ? String.Empty : string.Format("{0}", context.User.Single(u => u.Id == lot.LastChangedObjectsUserId.ToString()).UserName);

        }

        public string LotStatus { get; set; }

        public long Id { get; set; }

        public int Number { get; set; }

        public decimal Sum { get; set; }

        public string SourceOfFinancing { get; set; }

        public List<FundingJson> FundingList { get; set; }

        public string LastChangedUser { get; set; }

        public string LastChangedUser_UserName { get; set; }
        public string LastChangedObjectsUser_UserName { get; set; }

        public string LastChangedObjectsUser { get; set; }
    }
}