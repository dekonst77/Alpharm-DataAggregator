using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier.AddingDOPMonitoringDatabase
{
    /// <summary>
    /// результат проц-ры[GoodsClassifier].[GetDOPBlockingForMonitoringDatabase]
    /// </summary>
    public class GetDOPBlockingForMonitoringDatabase_Result
    {
        [Key]
        public long GoodsId { get; set; }
        public long GoodsTradeNameId { get; set; }
        public string GoodsTradeName { get; set; }
        public string GoodsDescription { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string OwnerTradeMarkKey { get; set; }
        public string OwnerTradeMark { get; set; }
        public long PackerId { get; set; }
        public string PackerKey { get; set; }
        public string Packer { get; set; }
        public long GoodsCategoryId { get; set; }
        public string GoodsCategoryName { get; set; }
        public Nullable<long> BrandId { get; set; }
        public string Brand { get; set; }
        public long ClassifierId { get; set; }
        public Nullable<long> BlockingForMonitoringId { get; set; }
        public Nullable<bool> Status { get; set; }
        public string StatusDesc { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> BlockTypeId { get; set; }
        public string BlockTypeName { get; set; }
        public string BlockTypeDescription { get; set; }
    }

}
