using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("SearchTask", Schema = "rts")]
    public class RTSSearchTask
    {
        public long Id { get; set; }
        public DateTime DatePublishStart { get; set; }
        public DateTime DatePublishEnd { get; set; }
        public int NextPage { get; set; }
        public int StatusId { get; set; }
        public DateTime? LastTryLoad { get; set; }
        public DateTime? LastReset { get; set; }
        public string ErrorMessage { get; set; }
    }
}
