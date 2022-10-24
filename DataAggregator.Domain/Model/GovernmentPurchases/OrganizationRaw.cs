using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("OrganizationRaw", Schema = "dbo")]
    public class OrganizationRaw
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public long? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public bool IsTrash { get; set; }
        public Int16? UserId { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}