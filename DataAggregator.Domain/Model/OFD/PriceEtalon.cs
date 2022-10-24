using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.OFD
{
    [Table("Price_Etalon", Schema = "dbo")]
    public class PriceEtalon
    {
        public int Id { get; set; }

        public DateTime Period { get; set; }

        public long ClassifierId { get; set; }

        public decimal Price { get; set; }

        public DateTime? DateUpdate { get; set; }

        public Guid? UserIdUpdate { get; set; }

    }
}
