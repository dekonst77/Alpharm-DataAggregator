using System;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.CalculatedDataEditor
{
    public class Filter
    {
        public string PurchaseId { get; set; }
        public string DrugId { get; set; }
        public string ClassifierId { get; set; }
        public string OwnerTradeMark { get; set; }
        public string OwnerTradeMarkId { get; set; }
        public string Packer { get; set; }
        public string PackerId { get; set; }
        public string PurchaseNumber { get; set; }
        public string INNGroup { get; set; }
        public DateTime PurchaseDateBeginStartValue { get; set; }
        public DateTime PurchaseDateBeginEndValue { get; set; }
        public string DrugTradeName { get; set; }
        public Byte[] SelectedNatureIds { get; set; }
        public Byte[] SelectedCategoryIds { get; set; }
        public string DrugDescription { get; set; }
        public string ObjectReadyName { get; set; }
        public string GoodsCategoryName { get; set; }
        public string[] SelectedFederalDistrictNames { get; set; }
        public bool IncludePurchases { get; set; }
        public string[] SelectedFederationSubjectNames { get; set; }
        public bool IncludeContracts { get; set; }

        public bool VNC { get; set; }
    }
}