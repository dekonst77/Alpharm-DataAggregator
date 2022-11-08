using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.LPU
{
    [Table("LPU_PointId")]
    public class LPUPoint
    {
        [Key]
        [Column("LPU_PointId")]       
        public int Id { get; set; }
        public int? ActualId { get; set; }
        public string BricksId { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        [Column("Post_Index")]
        public string Address_index { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_room_area { get; set; }
        public string Address_koor { get; set; }
        public decimal? Address_koor_lat { get; set; }
        public decimal? Address_koor_long { get; set; }
    }
}
