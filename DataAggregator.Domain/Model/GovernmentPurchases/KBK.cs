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
    [Table("KBK", Schema = "dbo")]
    public class KBK
    {
        [Key]
        [Column(Order = 1)]
        public string Id { get; set; }
        
        [Key]
        [ForeignKey("KBK_Main_Rasp")]
        [Column(Order = 10)]
        public string Customer_Bricks_L3 { get; set; }

        [ForeignKey("KBK_Main_Rasp")]
        
        [Column(Order = 2)]
        public string Main_Rasp { get; set; }
        
        public virtual KBK_Main_Rasp KBK_Main_Rasp { get; set; }
        [ForeignKey("Main_Rasp,Customer_Bricks_L3,ZS")]
        public virtual  KBK_ZS KBK_ZS { get; set; }

        public string Razdel { get; set; }
        [ForeignKey("Razdel")]
        public KBK_Razdel KBK_Razdel { get; set; }

        public string Razdel2 { get; set; }
        [ForeignKey("Razdel2")]
        public virtual KBK_Razdel2 KBK_Razdel2 { get; set; }

      //  [ForeignKey("KBK_ZS")]
        [Column(Order = 20)]
        public string ZS { get; set; }

        public string KodVidRashod { get; set; }
        [ForeignKey("KodVidRashod")]
        public virtual KBK_KodVidRashod KBK_KodVidRashod { get; set; }

        public bool IsUse { get; set; }

        public Byte? NatureId { get; set; }
        [JsonIgnore, ForeignKey("NatureId")]
        public virtual Nature Nature { get; set; }

        public Int16? Nature_L2Id { get; set; }
        [JsonIgnore, ForeignKey("Nature_L2Id")]
        public virtual Nature_L2 Nature_L2 { get; set; }

        public string Comment { get; set; }


        [ForeignKey("Id,Customer_Bricks_L3")]
        public virtual KBK_FundingView KBK_FundingView { get; set; }

        //[ForeignKey("Id,Customer_Bricks_L3")]
        public virtual List<KBK_Funding> KBK_Funding { get; set; }
    }
    [Table("KBK_Main_Rasp", Schema = "dbo")]
    public class KBK_Main_Rasp
    {
        [Key]
        [Column(Order = 1)]
        public string Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Customer_Bricks_L3 { get; set; }

        public string Value { get; set; }

        public string Bricks_L3 { get; set; }
        [ForeignKey("Bricks_L3")]
        public GS_Bricks_L3 GS_Bricks_L3 { get; set; }
    }
    [Table("KBK_ZS", Schema = "dbo")]
    public class KBK_ZS
    {
        [Key]
        [Column(Order = 2)]
        public string Main_Rasp { get; set; }
        [Key]
        [Column(Order = 10)]
        public string Customer_Bricks_L3 { get; set; }
        [Key]
        [Column(Order = 20)]
        public string ZS { get; set; }
        public string ZS_M1 { get; set; }
        public string ZS_M2 { get; set; }
        public string ZS_MM { get; set; }
        public string ZS_Napr { get; set; }
        public string ZS_M1_Value { get; set; }
        public string ZS_M2_Value { get; set; }
        public string ZS_MM_Value { get; set; }
        public string ZS_Napr_Value { get; set; }
    }
    [Table("KBK_Funding", Schema = "dbo")]
    public class KBK_Funding
    {
        [Key]
        [Column(Order = 1)]
        public string KBKId { get; set; }
        [Key]
        [Column(Order = 10)]
        public string Customer_Bricks_L3 { get; set; }
        [Key]
        [Column(Order = 35)]
        public Byte FundingId { get; set; }

        [JsonIgnore, ForeignKey("FundingId")]
        public Funding Funding { get; set; }

        [JsonIgnore, ForeignKey("KBKId,Customer_Bricks_L3")]
        public virtual KBK KBK { get; set; }
    }
    [Table("KBK_FundingView", Schema = "dbo")]
    public class KBK_FundingView
    {
        [Key]
        [Column(Order = 1)]
        public string KBKId { get; set; }
        [Key]
        [Column(Order = 10)]
        public string Customer_Bricks_L3 { get; set; }

        
        public string Value { get; set; }
    }
    [Table("KBK_Razdel", Schema = "dbo")]
    public class KBK_Razdel
    {
        [Key]
        public string Id { get; set; }

        public string Value { get; set; }
    }
    [Table("KBK_Razdel2", Schema = "dbo")]
    public class KBK_Razdel2
    {
        [Key]
        public string Id { get; set; }

        public string Value { get; set; }
    }
    [Table("KBK_KodVidRashod", Schema = "dbo")]
    public class KBK_KodVidRashod
    {
        [Key]
        public string Id { get; set; }

        public string Value { get; set; }
    }
}
