using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer
{
    [Table("SynManufacturer", Schema = "SearchTerms")]
    public class SynManufacturer
    {
        public long Id { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public string Value { get; set; }

        public bool IsForced { get; set; }

        public int Count { get; set; }

        public bool IsDictionary { get; set; }
    }
}
