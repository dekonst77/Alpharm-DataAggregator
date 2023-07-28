﻿using System;
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
        public int? CommentStatusId { get; set; }
        public string CommentStatusManual { get; set; }
        public decimal? TransferPrice { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? UserId { get; set; }
        public decimal? DeviationPercent { get; set; }
        public int? PriceDiff { get; set; }
    }
}