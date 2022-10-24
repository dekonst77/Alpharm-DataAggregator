using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GRLS
{
    //[Table("ProductionStage", Schema = "dbo")]
    public class ProductionStage
    {
        public long Id { get; set; }
        public string StageNumber { get; set; }
        public string StageName { get; set; }
        public string Manufacturer { get; set; }
        public string Country { get; set; }
        public long DrugInfoId { get; set; }

        public virtual DrugInfo DrugInfo { get; set; }

    }
}