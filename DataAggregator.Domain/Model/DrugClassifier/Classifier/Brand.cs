
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("Brand", Schema = "Classifier")]
    public class Brand : Common.DictionaryItem
    {
        public bool UseGoodsClassifier { get; set; }
        public bool UseClassifier { get; set; }
    }
}
