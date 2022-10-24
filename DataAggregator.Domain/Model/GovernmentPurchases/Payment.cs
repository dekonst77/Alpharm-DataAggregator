using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataAggregator.Domain.Model.GovernmentPurchases
{


    [Table("Payment", Schema = "dbo")]
    public class Payment
    {
        [Key]
        public long Id { get; set; }

        public long PurchaseId { get; set; }

        public string KBK { get; set; }

        public string KOSGU { get; set; }
        public decimal? Sum { get; set; }

        public long? PaymentYearId { get; set; }
        public long? PaymentTypeId { get; set; }

        public virtual Purchase Purchase { get; set; }

        public virtual PaymentYear PaymentYear { get; set; }

        public virtual PaymentType PaymentType { get; set; }
    }
    [Table("PaymentType", Schema = "dbo")]
    public class PaymentType
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
