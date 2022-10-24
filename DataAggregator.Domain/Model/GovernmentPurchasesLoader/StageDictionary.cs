using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Stage", Schema = "dict")]
    public class StageDictionary
    {
        public Byte Id { get; set; }
        public string Name { get; set; }
        public Byte StageId { get; set; }
    }
}