using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DataAggregator
{
    public abstract class ErrorLog
    {
        public long Id { get; set; }

        public string Project { get; set; }

        public string Controller { get; set; }

        public string Method { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public string AdditionalInfo { get; set; }

        public string Description { get; set; }
        
        public Guid? UserId { get; set; }
    }

    [Table("UserViewAll", Schema = "dbo")]
    public class UserViewAll
    {
        [Key]
        public string Id { get; set; }
        public Int16 UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}
