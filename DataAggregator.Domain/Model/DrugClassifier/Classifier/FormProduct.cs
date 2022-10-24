using System.Collections.Generic;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class FormProduct : Common.DictionaryItem
    {
        [JsonIgnore]
        public virtual IList<SynFormProduct> SynFormProduct { get; set; } 
    }
}