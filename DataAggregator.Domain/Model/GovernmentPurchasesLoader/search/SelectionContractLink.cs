using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.search
{
    [Table("SelectionContractLink", Schema = "search")]
    public class SelectionContractLink
    {      
        public long Id { get; set; }     
        public string PurchaseNumber { get; set; }
        public string ReestrNumber { get; set; }
        public int StatusId { get; set; }
        public string PurchaseUrl { get; set; }
        public long ContractSearchTaskId { get; set; }
        public string ErrorMessage { get; set; }
       
    }
}
