using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.proxy
{
    [Table("ErrorLog", Schema = "proxy")]
    public class ProxyErrorLog
    {
        public long Id { get; set; }
        public long ProxyId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public string ErrorMessage { get; set; }
        public string Url { get; set; }
       

    }
}
