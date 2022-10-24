using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("Customer", Schema = "rts")]
    public class RTSCustomer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string INN { get; set; }
        public string Address { get; set; }
    }
}
