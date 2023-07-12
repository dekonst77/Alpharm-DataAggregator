using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

namespace DataAggregator.Domain.Model.DrugClassifier.GTIN
{


    [Table("GTINs", Schema = "gtin")]
    public class GTINs
    {
        [Key]
        public long Id { get; set; }
        public long? SourceId { get; set; }
        public long? GTIN            { get; set; }
        public long? ClassifierId    { get; set; }
        public int? IsValid          { get; set; }
        public bool? IsActive         { get; set; }
        public bool? IsCheck          { get; set; }
        public DateTime? DtAdd       { get; set; }
        public int? Status { get; set; }
        [ForeignKey("SourceId")]
        public virtual Systematization.Source Source { get; set; }
    }
    public class GTINs_View
    {
        [Key]
        public long Id { get; set; }
        public long? SourceId { get; set; }
        public string Source { get; set; }
        public long? GTIN { get; set; }
        public long? ClassifierId { get; set; }
        public Int16? IsValid { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsCheck { get; set; }
        public DateTime? DtAdd { get; set; }
        public Int16? Status { get; set; }
        public int? DrugId { get; set; }
        public int? GoodsId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public int? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public int? PackerId { get; set; }
        public string Packer { get; set; }
        public string DrugType { get; set; }
        public bool? Used { get; set; }
        public DateTime? LastChangedDate { get; set; }
        public Guid? LastChangedUserId { get; set; }
        public string UserName { get; set; }
        public string OperatorComment { get; set; }
        public string Search { get; set; }


    }
    public class GTINs_Filter
    {
     
        public string Id { get; set; }
        public long? SourceId { get; set; }
        public string Source { get; set; }
        public string GTIN { get; set; }
        public string ClassifierId { get; set; }
        public Int16? IsValid { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsCheck { get; set; }
        public Int16? Status { get; set; }
        public string DrugId { get; set; }
        public string GoodsId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public string PackerId { get; set; }
        public string Packer { get; set; }
        public string DrugType { get; set; }
        public bool? Used { get; set; }
        public DateTime? LastChangedDate { get; set; }
        public Guid? LastChangedUserId { get; set; }
        public string UserName { get; set; }
        public string OperatorComment { get; set; }
        public string Search { get; set; }


    }

}
