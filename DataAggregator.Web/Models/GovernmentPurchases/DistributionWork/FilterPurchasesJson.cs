using System;

namespace DataAggregator.Web.Models.GovernmentPurchases.DistributionWork
{
    public class FilterPurchasesJson
    {
        public string DateBegin_Start { get; set; }
        public string DateBegin_End { get; set; }
        public string DateEnd_Start { get; set; }
        public string DateEnd_End { get; set; }
        public string PurchaseDateCreate_Start { get; set; }
        public string PurchaseDateCreate_End { get; set; }
        public bool IsAssignedToUser { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Byte? PurchaseClassId { get; set; }
        public string PurchaseClassUserId { get; set; }
        public bool isCheckTZ { get; set; }
        public bool withPtotokol { get; set; }
    }
}