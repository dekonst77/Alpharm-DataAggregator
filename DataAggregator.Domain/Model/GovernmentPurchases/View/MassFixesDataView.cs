using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class MassFixesDataView
    {
        [Key]
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public long? LotId { get; set; }
        public long? porID { get; set; }
        public long? contractId { get; set; }
        public int? LotNumber { get; set; }    
        public decimal? LotSum { get; set; }
        public string PurchaseName { get; set; }
        public string Comment { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string NatureName { get; set; }
        public string Nature_L2Name { get; set; }
        public string CategoryName { get; set; }
        public string LawTypeName { get; set; }
        public string SourceOfFinancing { get; set; }
        public string FundingNames { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryTimePeriod_text { get; set; }
        public string URL { get; set; }
        public string LotStatusName { get; set; }

        public long? CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerINN { get; set; }
        public string CustomerOrganizationType { get; set; }
        public string CustomerFederationSubject { get; set; }

        public long? ReceiverId { get; set; }
        public string ReceiverFullName { get; set; }
        public string ReceiverShortName { get; set; }
        public string ReceiverINN { get; set; }
        public string ReceiverOrganizationType { get; set; }
        public string ReceiverFederationSubject { get; set; }

        public string KBKs { get; set; }
        public string contract_KBKs { get; set; }

        public string PurchaseClassName { get; set; } 
        public string LastChangedUser_Purchase { get; set; } 
        public string LastChangedUser_Lot { get; set; } 
        public string LastChangedUser_PurchaseObjectReady { get; set; } 

    }

    public class DeliveryTimeSetView
    {
        //[Key]
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public string URL { get; set; }
        public string DeliveryTime { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }        
        public string CustomerFederationSubject { get; set; }
        public string CustomerShortName { get; set; }
        public string PurchaseName { get; set; }
        public decimal LotSum { get; set; }
        public DateTime? ContractDateBegin { get; set; }
        public DateTime? ContractDateEnd { get; set; }
        public int? idDTI { get; set; }
        public DateTime? DateStartDTI { get; set; }
        public DateTime? DateEndDTI { get; set; }
        public int? Count { get; set; }
        public Byte? DeliveryTimePeriodId { get; set; }
        public int? count_DTI { get; set; }
        public int? DayDelta { get; set; }
    }
}
