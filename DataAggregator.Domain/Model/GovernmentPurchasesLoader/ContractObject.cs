using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("ContractObject", Schema = "buffer")]
    public class ContractObject
    {
      public long Id         {get;set;}
      public string Name { get; set; }
      public string OKPD       {get;set;}
      public string Unit { get; set; }
      public decimal Amount     {get;set;}
      public decimal Price { get; set; }
      public decimal Sum { get; set; }
      public long ContractId {get;set;}

      public virtual Contract Contract { get; set; }
    }
}