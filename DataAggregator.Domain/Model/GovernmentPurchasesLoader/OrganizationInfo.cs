using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("OrganizationInfo", Schema = "org")]
    public class OrganizationInfo
    {
        public long Id { get; set; }
        public string url { get; set; }
        public string RegistrationStatus { get; set; }
        public string RegistrationDate { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string OGRN { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string TaxRegistrationDate { get; set; }
        public string IKOCode { get; set; }
        public string IKODate { get; set; }
        public string LocationAddress { get; set; }
        public string OKVED { get; set; }
        public string AdditionalOKVED { get; set; }
        public string OKPO { get; set; }
        public string OKATO { get; set; }
        public string OKTMOCode { get; set; }
        public string OKFC { get; set; }
        public string OKOPF { get; set; }
        public string PostAddress { get; set; }
        public string Mail { get; set; }
        public string Site { get; set; }
        public string ContactPerson { get; set; }
        public string ContactMail { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string OrganizationAuthority { get; set; }
        public string OrganizationType { get; set; }
        public string SubKPP { get; set; }
        public string SubFullName { get; set; }
        public string SubShortName { get; set; }
        public string SubIKOCode { get; set; }
        public string SubIKODate { get; set; }
        public bool? Is223 { get; set; }
        public string OrganizationLevel { get; set; }
        public string SPZChangingSource { get; set; }
        public string SPZChangingDate { get; set; }
        public string IKUCode { get; set; }
        public string IKUDate { get; set; }
        public string BudgetCode { get; set; }
        public string BudgetName { get; set; }
        public string AdministrativeAffiliationSPZ { get; set; }
        public string AdministrativeAffiliationName { get; set; }
        public string ParentSPZ { get; set; }
        public string ParentName { get; set; }
        public string OKOGU { get; set; }
        public string OKTMOName { get; set; }
        public string CodeSPZ { get; set; }
        public long? RegionId { get; set; }
        public int? IsAnalyze { get; set; }
        public int? index { get; set; } 
    }
}