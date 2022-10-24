using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ContractPaymentStage", Schema = "buffer")]
    public class ContractPaymentStage
    {
      public long Id {get;set;}
      public long ContractId {get;set;}
      public long PaymentTypeId { get; set; }
      public string StageDate {get;set;}
      public string Code  {get;set;}
      public decimal Sum  {get;set;}

      public virtual Contract Contract { get; set; }
      
    }
}