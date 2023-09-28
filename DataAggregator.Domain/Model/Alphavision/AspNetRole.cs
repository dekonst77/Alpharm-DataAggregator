using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Alphavision
{
    [Table("AspNetRoles", Schema = "dbo")]
    public class AspNetRole
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }

        public ICollection<AspNetUserRole> UserRoles { get; set; }
    }

}
