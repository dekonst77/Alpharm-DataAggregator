using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

namespace DataAggregator.Domain.Model.DrugClassifier.Rasp
{
    [Table("Data", Schema = "rasp")]
    public class Data
    {
        [Key]
        public int Id { get; set; }
        public int RaspredelenieId { get; set; }
        public long ClassifierId_Before { get; set; }
        public long ClassifierId_After { get; set; }
        public Int16? UserId { get; set; }

        [ForeignKey("RaspredelenieId")]
        public virtual Raspredelenie Raspredelenie { get; set; }
    }
    [Table("DataView", Schema = "rasp")]
    public class DataView
    {
        [Key]
        public int Id { get; set; }
        public int RaspredelenieId { get; set; }
        public long ClassifierId_Before { get; set; }
        public long ClassifierId_After { get; set; }
        public Int16? UserId { get; set; }
        public string UserName { get; set; }

        public string TradeName_Before { get; set; }
        public string DrugDescription_Before { get; set; }
        public string INNGroup_Before { get; set; }
        public string OwnerTradeMark_Before { get; set; }
        public string Packer_Before { get; set; }
        public string TradeName_After { get; set; }
        public string INNGroup_After { get; set; }
        public string DrugDescription_After { get; set; }
        public string OwnerTradeMark_After { get; set; }
        public string Packer_After { get; set; }

        public bool? Used_After { get; set; }
        public bool? Used_Before { get; set; }
    }
    [Table("Tables", Schema = "rasp")]
    public class Tables
    {
        [Key]
        public Byte Id { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
    }
    [Table("Raspredelenie", Schema = "rasp")]
    public class Raspredelenie
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_Begin { get; set; }
        public DateTime Date_End { get; set; }
        public Byte TableId { get; set; }
        public DateTime Date_Create { get; set; }
        public string Region { get; set; }
        public bool IsEnd { get; set; }

        [ForeignKey("TableId")]
        public virtual Tables Table { get; set; }
    }

    
}
