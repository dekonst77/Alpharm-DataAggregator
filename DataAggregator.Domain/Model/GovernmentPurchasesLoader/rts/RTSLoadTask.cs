using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("LoadTask", Schema = "rts")]
    public class RTSLoadTask
    {
        public long Id { get; set; }
        public string PurchaseNumber { get; set; }
        public string Url { get; set; }
        public int StatusId { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? LastTryLoad { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
