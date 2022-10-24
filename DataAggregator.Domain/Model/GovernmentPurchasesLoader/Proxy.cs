using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Proxy", Schema = "dbo")]
    public class Proxy
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime? LastError { get; set; }
    }
}