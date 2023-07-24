using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassifierReport", Schema = "report")]
    public partial class ClassifierReport
    {
        [Key]
        public byte Id { get; set; }
        public string ReportCode { get; set; }
    }
}
