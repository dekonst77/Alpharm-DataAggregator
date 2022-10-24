using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.GRLS
{
    //[Table("Drug", Schema = "dbo")]
    public class Drug
    {
        public long Id { get; set; }
        public string ExpDate { get; set; }
        public string FormProduct { get; set; }
        public string Dosage { get; set; }
        public long DrugInfoId { get; set; }

        public virtual DrugInfo DrugInfo { get; set; }
     
    }
}