using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class ProductionInfoEtalonPriceView
    {
        [Key]
        public long ProductionInfoId { get; set; }

        public decimal Price { get; set; }
    }
}
