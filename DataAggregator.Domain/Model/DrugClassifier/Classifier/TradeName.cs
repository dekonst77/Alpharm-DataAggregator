
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("TradeName", Schema = "Classifier")]
    public class TradeName : Common.DictionaryItem
    {
        [JsonIgnore]
        public virtual IList<SynTradeName> SynTradeName { get; set; } 
    }
}