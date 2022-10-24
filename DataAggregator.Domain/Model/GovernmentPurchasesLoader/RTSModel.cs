using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
       [Table("RTSModel", Schema = "dbo")]
    public class RTSModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
       
        public string Datatenderid { get; set; }
        public string PurchaseId { get; set; }
        public DateTime Date { get; set; }
    }
}
