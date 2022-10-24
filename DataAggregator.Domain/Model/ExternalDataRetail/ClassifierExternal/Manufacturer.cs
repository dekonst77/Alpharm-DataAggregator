using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.ExternalDataRetail.ClassifierExternal
{
     [Table("Manufacturer", Schema = "ClassifierExternal")]
    public class Manufacturer : Common.DictionaryItem
    {
        public string Key { get; set; }
        public long? KeyOrder { get; set; }
    }
}