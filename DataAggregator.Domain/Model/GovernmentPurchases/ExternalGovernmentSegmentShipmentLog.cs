using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{

    [Table("CreateExternalShipment", Schema = "logs")]
    public class CreateExternalShipmentLog
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Step { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
    }
}
