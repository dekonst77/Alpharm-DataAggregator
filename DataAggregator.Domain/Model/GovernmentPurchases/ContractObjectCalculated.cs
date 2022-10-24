using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractObjectCalculated
    {
        public long Id { get; set; }
        public long ContractObjectReadyId { get; set; }
        public decimal Amount { get; set; }
        public decimal? RecoveredPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sum { get; set; }
        public long? DrugId { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public long? PackerId { get; set; }
        public bool? IsOther { get; set; }
        public Int16 type { get; set; }
        public Byte? RecoveryTypeId { get; set; }
        public decimal? Coefficient { get; set; }
    }
}
