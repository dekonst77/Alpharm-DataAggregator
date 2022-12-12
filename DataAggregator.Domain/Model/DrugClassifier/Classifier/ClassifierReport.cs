using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassifierReport", Schema = "report")]
    public partial class ClassifierReport
    {
        public byte Id { get; set; }
        public string ReportCode { get; set; }
    }
}
