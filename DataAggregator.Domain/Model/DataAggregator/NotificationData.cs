using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DataAggregator
{
    [Table("NotificationGroups", Schema = "dbo")]
    public class NotificationGroups
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("NotificationGroupUsers", Schema = "dbo")]
    public class NotificationGroupUsers
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
