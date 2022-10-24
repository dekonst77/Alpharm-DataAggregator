using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.InputData
{
    [Table("HtmlSource", Schema = "InputData")]
    public class HtmlSource
    {
        public long Id { get; set; }

        public string Html { get; set; }

        public int Code { get; set; }

        public string ValueCode { get; set; }

        public string Url { get; set; }
    }
}
