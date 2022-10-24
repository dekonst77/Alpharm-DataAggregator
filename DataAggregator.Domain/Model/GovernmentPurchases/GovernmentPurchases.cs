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
    [Table("AutoNature_Text", Schema = "dbo")]
    public class AutoNature_Text
    {
        [Key]
        public int Id { get; set; }

        public string Customer_Bricks_L3 { get; set; }
        public string Value { get; set; }
        public Byte? NatureId { get; set; }
        public Int16? Nature_L2Id { get; set; }
        public Byte? FundingId { get; set; }
        public string Comment { get; set; }
        public bool IsInName { get; set; }

    }
}
