using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("Manufacturer", Schema = "Classifier")]
    public class Manufacturer : Common.DictionaryItem
    {
        //public string Key { get; set; }
        public string Value_eng { get; set; }
        //public long? KeyOrder { get; set; } 
        public long? CorporationId { get; set; }
        public virtual Corporation Corporation { get; set; }

        public long? CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
    [Table("ManufacturerClear", Schema = "Classifier")]
    public class ManufacturerClear
    {
        [Key]
        public long Id { get; set; }
        public string Value { get; set; }
    }

}