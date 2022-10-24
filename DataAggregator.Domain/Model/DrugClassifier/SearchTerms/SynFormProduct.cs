using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.SearchTerms
{
    [Table("SynFormProduct", Schema = "SearchTerms")]
    public class SynFormProduct : AbstractSynonym
    {
        [ForeignKey("OriginalId")] 
        public virtual FormProduct FormProduct { get; set; }
    }
}
