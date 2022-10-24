using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("ContractAveragePrice", Schema = "calc")]
    public class ContractAveragePrice
    {
        public long Id { get; set; }
        public long RegionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long ClassifierId { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime LastChangedDate { get; set; }
        public Guid? LastChangedUserId { get; set; }
    }
}
