using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractPaymentStage
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public string KBK { get; set; }
        public string StageDate { get; set; }
        public decimal Sum { get; set; }
        public long PaymentTypeId { get; set; }
        public DateTime Date { get; set; }
    }
    public class Contract_check_ContractPaymentStage
    {
        public long Id { get; set; }
        public long ContractId { get; set; }
        public string KBK { get; set; }
        public string StageDate { get; set; }
        public decimal Sum { get; set; }
        public long PaymentTypeId { get; set; }
        public DateTime Date { get; set; }
    }
}
