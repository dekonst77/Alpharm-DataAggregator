using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class WrongPricesView
    {
        [Key]
        public long ObjectCalculatedId { get; set; }
        public int ObjectTypeId { get; set; }
        public string ObjectType { get; set; }
        public long PurchaseId { get; set; }

        public long PurchaseObjectCalculatedId { get; set; }
        public long ContractObjectCalculatedId { get; set; }
        
        public string PurchaseNumber { get; set; }
        public DateTime DateBegin { get; set; }        
        public string NatureName { get; set; }
        public decimal? LotSum { get; set; }
        public int? ClassifierId { get; set; }
        public int? DrugId { get; set; }        


        public string InnGroup { get; set; }
        public decimal? FDPriceCoefficient { get; set; }
        public decimal? ObjectCalculatedPrice { get; set; }
        public decimal? FDAveragePrice { get; set; }
        public string ObjectReadyUnit { get; set; }
        public decimal ObjectReadyAmount { get; set; }
        public string ObjectReadyName { get; set; }

        public bool VNC { get; set; }
        public decimal? kofPriceGZotkl { get; set; }
    }
}
