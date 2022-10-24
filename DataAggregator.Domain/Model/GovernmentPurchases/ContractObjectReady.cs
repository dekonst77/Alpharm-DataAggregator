using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractObjectReady
    {
        public long Id {get; set;}
        public string Name {get; set;}
        public string Manufacturer {get; set;}
        public string OKPD {get; set;}
        public string Unit {get; set;}
        public decimal Amount {get; set;}
        public decimal? Price {get; set;}
        public decimal? Sum {get; set;}
        public long? ContractId {get; set;}
        [JsonIgnore]
        public virtual Contract Contract { get; set; }
        public long? DrugRawId {get; set;}
        public long? DrugClassifierId {get; set;}
        public decimal? AmountCorrected {get; set;}
        public decimal? PriceCorrected {get; set;}
        public decimal? SumCorrected {get; set;}
        public long? ClassifierId { get; set; }
        public bool VNC { get; set; }
    }

    public class ContractObjectReady_History
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string OKPD { get; set; }
        public string Unit { get; set; }
        public decimal Amount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sum { get; set; }
        public long? ContractId { get; set; }
        public DateTime DT { get; set; }
    }
    [Table("contract_stage_Objects_View", Schema = "dbo")]
    public class contract_stage_Objects_View
    {
        [Key]
        public int contractQuantityId { get; set; }
        public long ContractId { get; set; }
        public string doc { get; set; }
        public DateTime date_end { get; set; }
        public decimal sum_go { get; set; }
        public decimal sum_pay { get; set; }
        public string tovar { get; set; }
        public string lek_form { get; set; }
        public string inn_name { get; set; }
        public string dosage { get; set; }
        public decimal price { get; set; }
        public string seria { get; set; }
        public DateTime date_expiration { get; set; }
        public string EI { get; set; }
        public decimal? amount_ei_in { get; set; }
        public decimal? amount { get; set; }
        public string country_reg { get; set; }
        public decimal? sumObject { get; set; }
    }
}
