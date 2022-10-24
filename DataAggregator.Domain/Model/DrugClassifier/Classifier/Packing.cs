using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class Packing
    {
        [Key]
        public long? Id { get; set; }
        public string Value { get; set; }
        //public string Value_Eng { get; set; }
    }
}