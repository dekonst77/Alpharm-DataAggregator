using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PostIndexes", Schema = "org")]
    public class PostIndexes
    {
        [Key]
        public string INDEX { get; set; }
        public string OPSNAME { get; set; }
        public string OPSTYPE { get; set; }
        public string OPSSUBM { get; set; }
        public string REGION { get; set; }
        public string AUTONOM { get; set; }
        public string AREA { get; set; }
        public string CITY { get; set; }
        public string CITY_1 { get; set; }
        public System.DateTime? ACTDATE { get; set; }
        public string INDEXOLD { get; set; } 
    }
}