using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("VEDPeriod", Schema = "Classifier")]
    public class VEDPeriod
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int YearStart { get; set; }
        public int MonthStart { get; set; }
        public int YearEnd { get; set; }
        public int MonthEnd { get; set; }
        
    }
}