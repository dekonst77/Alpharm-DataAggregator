using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.EtalonPrice
{
    [Table("CommentStatuses", Schema = "EtalonPrice")]
    public class CommentStatuses
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("MainData", Schema = "EtalonPrice")]
    public class MainData
    {
        [Key]
        public long Id { get; set; }
        public long ClassifierId { get; set; }
        public int? CommentStatusId { get; set; }
        public string CommentStatusManual { get; set; }
        public decimal? TransferPrice { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? UserId { get; set; }
        public decimal? DeviationPercent { get; set; }
        public int? PriceDiff { get; set; }
        public bool ForChecking { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }

    [Table("LinkedUserData", Schema = "EtalonPrice")]
    public class LinkedUserData
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [Table("SourceInfo", Schema = "EtalonPrice")]
    public class SourceInfo
    {
        [Key]
        public long Id { get; set; }
        public bool ForChecking { get; set; }
        public DateTime? DateModify { get; set; }
        public Guid? UserModify { get; set; }

    }
}