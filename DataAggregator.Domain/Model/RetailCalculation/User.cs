using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("UserView", Schema = "dbo")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }
}
