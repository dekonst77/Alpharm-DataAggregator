using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("Method", Schema = "dict")]
    public class MethodDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Byte? MethodId { get; set; }
    }
}