using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Purchase
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long? CustomerId { get; set; }
        public Byte LawTypeId { get; set; }
        public Byte MethodId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string Name { get; set; }
        public Byte? StageId { get; set; }
        public string StandartContractNumber { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? DateEndFirstParts { get; set; }
        public string URL { get; set; }
        //public long? PlanPositionId { get; set; }
        public DateTime DateCreate { get; set; }
        
        public Guid? AssignedToUserId { get; set; }
        public Guid? ContractAssignedToUserId { get; set; }
        public bool HigherPriority { get; set; }
        public Byte PurchaseClassId { get; set; }
        public Guid? PurchaseClassUserId { get; set; }

        public string DeliveryTime { get; set; }
        public string WhoIsPurchasing { get; set; }
        public string PriceJustification { get; set; }
        public Byte SourceId { get; set; }

        public Byte? CategoryId { get; set; }
        public Byte? NatureId { get; set; }
        public Int16? Nature_L2Id { get; set; }
        public bool? IsPriceByPart { get; set; }
        public Guid? LastChangedUserId { get; set; }
        
        public DateTime? LastChangedDate { get; set; }

        public bool UseContractData { get; set; }
        public string ConclusionReason { get; set; }

        [StringLength(300)]
        public string Comment { get; set; }

        [JsonIgnore]
        public virtual Organization Customer { get; set; }

        [JsonIgnore]
        public virtual LawType LawType { get; set; }

        [JsonIgnore]
        public virtual Source Source { get; set; }

        [JsonIgnore]
        public virtual Method Method { get; set; }

        [JsonIgnore]
        public virtual Stage Stage { get; set; }

        [JsonIgnore]
        public virtual IList<Lot> Lot { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual Nature Nature { get; set; }
        [JsonIgnore]
        public virtual Nature_L2 Nature_L2 { get; set; }
        [JsonIgnore]
        public virtual IList<DeliveryTimeInfo> DeliveryTimeInfo { get; set; }

        [JsonIgnore]
        public virtual IList<Payment> Payment { get; set; }

       // [JsonIgnore]
      //  public virtual IList<StatusHistory> StatusHistory { get; set; }

        [JsonIgnore]
        public virtual PurchaseClass PurchaseClass { get; set; }


        public virtual IList<PurchaseNatureMixed> PurchaseNatureMixed { get; set; } 
    }

    public class PlanG
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public int Number { get; set; }
        public decimal Sum { get; set; }
        public string Customer { get; set; }
        public string IKZ { get; set; }
        public string Place { get; set; }
        public string Plan_Url { get; set; }
        public string Plan_Id { get; set; }
        public long? Customer_id { get; set; }
    }
    public class PlanG_View
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public int Number { get; set; }
        public decimal Sum { get; set; }
        public string Customer { get; set; }
        public string IKZ { get; set; }
        public string Place { get; set; }
        public string Plan_Url { get; set; }
        public string Plan_Id { get; set; }
        public long? Customer_id { get; set; }
        public string ShortName { get; set; }
    }
}
