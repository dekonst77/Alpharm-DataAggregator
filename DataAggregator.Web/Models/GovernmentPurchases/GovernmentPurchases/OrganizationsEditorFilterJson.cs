using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class OrganizationsEditorFilterJson
    {
        public string Id { get; set; }
        public string Inn { get; set; }
        public string Text { get; set; }
        public DictionaryElementJson OrganizationType { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string[] SelectedFederalDistrictNames { get; set; }
        public string[] SelectedFederationSubjectNames { get; set; }
        public bool OnlyDrugsLinked { get; set; }
        public bool OnlyEmptyType { get; set; }
        public bool OnlyEmptyRegion { get; set; }
        public bool is_LO { get; set; }
        public bool is_CP { get; set; }
        public bool is_Actual { get; set; }
        public bool no_iin { get; set; }
    }
}