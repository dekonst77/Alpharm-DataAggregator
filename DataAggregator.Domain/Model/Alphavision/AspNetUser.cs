using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Alphavision
{
    [Table("AspNetUsers", Schema = "dbo")]
    public class AspNetUser
    {
        [Key]
        public string Id { get; set; } 
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public int? PostId { get; set; } 
        public bool ApiEnabled { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public Supplier Supplier { get; set; }
        public Post Post { get; set; }
        public ICollection<AspNetUserRole> UserRoles { get; set; }
    }
}
