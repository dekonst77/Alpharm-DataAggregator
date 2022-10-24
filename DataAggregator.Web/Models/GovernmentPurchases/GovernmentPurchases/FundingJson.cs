using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class FundingJson
    {
        public FundingJson()
        {
            
        }

        public FundingJson(Funding funding, Lot lot)
        {
            if (funding == null)
            {
                throw new ArgumentNullException("funding");
            }

            Id = funding.Id;
            InternalName = funding.InternalName;
            Name = funding.Name;
            Code = funding.Code;
            CanGetTransfer = funding.CanGetTransfer;
            IsBudget = funding.IsBudget;
            IsNotBudget = funding.IsNotBudget;
            IsTransfer = funding.IsTransfer;

            

            CheckedList = new List<CheckedFunding>();
            if (lot != null)
            {
                var checkedCount = lot.LotFunding.Count(lf => lf.Funding == funding);
                CheckedList.Add(new CheckedFunding() { Checked = checkedCount >= 1 });
                if (funding.CanGetTransfer)
                {
                    CheckedList.Add(new CheckedFunding() { Checked = checkedCount >= 2 });
                }
            }
            else
            {
                CheckedList.Add(new CheckedFunding() { Checked = false });
            }
        }
        public Byte Id { get; set; }

        public string InternalName { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool CanGetTransfer { get; set; }

        public bool IsBudget { get; set; }

        public bool IsNotBudget { get; set; }

        public bool IsTransfer { get; set; }

        public List<CheckedFunding> CheckedList { get; set; }
    }

    public class CheckedFunding
    {
        public bool Checked { get; set; }
    }
}