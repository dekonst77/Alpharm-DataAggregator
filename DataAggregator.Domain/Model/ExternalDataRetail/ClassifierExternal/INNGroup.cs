using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;


namespace DataAggregator.Domain.Model.ExternalDataRetail.ClassifierExternal
{
     [Table("INNGroup", Schema = "ClassifierExternal")]
    public class INNGroup
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}