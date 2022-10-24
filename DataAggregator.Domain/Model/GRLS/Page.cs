using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GRLS
{
    //[Table("Page", Schema = "dbo")]
    public class Page
    {
        public long Id {get; set; }
        public long Number {get; set; }
        public string Html {get; set; }
        public DateTime DateAdd {get; set; }
        public string ErrorMEssage { get; set; }
        public int AnalyzeId { get; set; }
        public string ErrorMessage { get; set; }
    }
}