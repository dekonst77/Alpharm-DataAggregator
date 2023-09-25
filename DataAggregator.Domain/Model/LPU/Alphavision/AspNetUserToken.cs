using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.LPU.Alphavision
{
    public class AspNetUserToken
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string LoginProvider { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
