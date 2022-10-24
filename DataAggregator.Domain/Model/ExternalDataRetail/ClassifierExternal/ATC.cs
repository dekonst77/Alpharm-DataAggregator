using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.Common;

namespace DataAggregator.Domain.Model.ExternalDataRetail.ClassifierExternal
{
    [Table("ATC", Schema = "ClassifierExternal")]
    public class Atc : HierarchicalDictionaryItem<Atc>
    {
         
    }
}