using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.OFD
{
    public class SourceInfoView_SP_Result
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ClassifierId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public bool IsSTM { get; set; }


        public Nullable<long> DrugClearId { get; set; }
        /// <summary>
        /// тип источника (1 - Исходники, 2 - Парсинг, 3 - ОФД, 4 - SellIn)
        /// </summary>
        public int? SourceTypeId { get; set; }
        public int? SourceId { get; set; }
        public string SourceName { get; set; }
        public decimal? Price  { get; set; }
        public string OriginalDrugName  { get; set; }
        public long? PharmacyId { get; set; }
        public bool ForChecking { get; set; }
        public string Manufacturer { get; set; }
    }
}
