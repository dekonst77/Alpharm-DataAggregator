
namespace DataAggregator.Domain.Model.Retail
{
    public class AggregatedRawDataByDrugClear
    {
        /// <summary>
        /// Год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Месяц
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Источник данных
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Id файла
        /// </summary>
        public long FileInfoId { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Исходное написание препарата
        /// </summary>
        public string Drug { get; set; }

        /// <summary>
        /// Исходное написание производителя
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Цена продаж в руб
        /// </summary>
        public decimal? SellingPriceNds { get; set; }

        /// <summary>
        /// Сумма продаж в руб
        /// </summary>
        public decimal? SellingSumNds { get; set; }

        /// <summary>
        /// Сумма продаж в уп
        /// </summary>
        public decimal? SellingCount { get; set; }

        /// <summary>
        /// Цена закупок в руб
        /// </summary>
        public decimal? PurchasePriceNds { get; set; }

        /// <summary>
        /// Сумма закупок в руб
        /// </summary>
        public decimal? PurchaseSumNds { get; set; }

        /// <summary>
        /// Сумма закупок в уп
        /// </summary>
        public decimal? PurchaseCount { get; set; }
    }
}