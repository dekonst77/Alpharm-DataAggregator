using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.LPU.Alphavision
{
    public class AspNetUser
    {
        [Key]
        public string Id { get; set; } 
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public int PostId { get; set; } 
        public bool ApiEnabled { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public virtual AspNetUserRole[] Roles { get; set; } = new AspNetUserRole[0];
    }
}
