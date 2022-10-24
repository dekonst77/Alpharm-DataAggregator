using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ProxyStatLoad", Schema = "dbo")]
    public class ProxyStatLoad
    {
      public long Id { get; set; }
      public long ProxyId { get; set; }
      public DateTime Date { get; set; }
      public bool IsLoad { get; set; }
      public bool Error { get; set; }
        public string ErrorMessage { get; set; }
    }
}