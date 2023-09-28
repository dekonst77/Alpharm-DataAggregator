using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractAndFulfilmentObjectReplace
    {
        public int Id { get; set; }
        public long contractQuantityId { get; set; }
        public long? ClassifierId { get; set; }
        public decimal? ObjectCalculatedAmount { get; set; }
        public decimal? ObjectCalculatedPrice { get; set; }
        [MaxLength(128)]
        public string UserGuid { get; set; }
        public DateTime EditDate { get; set; }
	    public int Status { get; set; }
    }
}
