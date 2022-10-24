using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("GoodsUserStat", Schema = "Stat")]
    public class GoodsUserStat
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int InWorkCount { get; set; }
        public int IsReadyCount { get; set; }
    }
}