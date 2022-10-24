namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class OrganizationFilterJson
    {
        public string INN { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }

        public bool Is_Customer { get; set; }
        public bool Is_Recipient { get; set; }
        public bool NotSet()
        {
            return string.IsNullOrEmpty(INN) && string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(Name);
        }
    }
}