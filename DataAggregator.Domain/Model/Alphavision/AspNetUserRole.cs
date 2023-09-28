using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Alphavision
{
    [Table("AspNetUserRoles", Schema = "dbo")]
    public class AspNetUserRole
    {
        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string UserId { get; set; }

        public AspNetUser User { get; set; }
        public AspNetRole Role { get; set; }
    }

}
