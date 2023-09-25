using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.LPU.Alphavision
{
    public class AspNetUserRole
    {
        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string UserId { get; set; }
        public virtual AspNetRole Role { get; set; }
    }

}
