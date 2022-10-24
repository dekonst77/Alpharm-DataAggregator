using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("TriggerLog", Schema = "logs")]
    public class TriggerLog
    {
        [Key]
        public long Id { get; set; }
        public long Purchase_Id { get; set; }
        public long Lot_Id { get; set; }
        public long Contract_Id { get; set; }

        public string Who { get; set; }
        public string What { get; set; }

        public DateTime When { get; set; }
    }
}
