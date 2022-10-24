namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class SupplierFilterModel
    {
        public long? Id { get; set; }
        public string KPP { get; set; }
        public string INN { get; set; }
        public string LocationAddress { get; set; }
        public string Name { get; set; }

        public bool NotSet()
        {
            return  string.IsNullOrEmpty(INN) && 
                    string.IsNullOrEmpty(LocationAddress) && 
                    string.IsNullOrEmpty(Name) &&
                    string.IsNullOrEmpty(KPP) && 
                    !Id.HasValue;
        }
    }
}