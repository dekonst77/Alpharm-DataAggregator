using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("PurchaseObjectReadyBulkInsert", Schema = "dbo")]
    public class PurchaseObjectReadyBulkInsert
    {
        public long Id { get; set; }
        public long PorId { get; set; }

        public long LotId { get; set; }

        public string Name { get; set; }

        public string OKPD { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }

        public long? ReceiverId { get; set; }
        public string ReceiverRaw { get; set; }

        public Guid GroupId { get; set; }

        public Guid UserId { get; set; }


    }
}