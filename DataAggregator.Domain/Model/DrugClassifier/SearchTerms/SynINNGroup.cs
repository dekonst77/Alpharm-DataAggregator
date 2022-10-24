using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.SearchTerms
{
    [Table("SynINNGroup", Schema = "SearchTerms")]
    public class SynINNGroup : AbstractSynonym
    {
        [ForeignKey("OriginalId")] 
        public virtual INNGroup INNGroup { get; set; }
    }
}
