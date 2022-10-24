using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.ExternalDataRetail.ClassifierExternal
{
    [Table("TradeName", Schema = "ClassifierExternal")]
    public class TradeName : Common.DictionaryItem
    {
       
    }
}