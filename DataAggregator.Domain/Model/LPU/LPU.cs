using DataAggregator.Domain.Model.GS;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAggregator.Domain.Model.LPU
{
    [Table("LPUId", Schema = "dbo")]
    public class LPU
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ActualId { get; set; }
        [Column("LPU_PointId")]
        public int LPUPointId { get; set; }
        public int OrganizationId { get; set; }

        public string Comment { get; set; }

        public int? DepartmentId { get; set; }

        public string Department { get; set; }
        // public virtual Department Department { get;set;}
        public int? TypeId { get; set; }
        public int? KindId { get; set; }
        public virtual Organization Organization {get;set;}
        public  virtual LPUPoint LPUPoint { get; set; }
    }
}
