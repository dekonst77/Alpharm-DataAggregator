namespace DataAggregator.Web.Models.Retail.SearchRawDataByClassifier
{
    public sealed class SearchRawDataByClassifierModel
    {
        public int YearStart { get; set; }
        public int MonthStart { get; set; }

        public int YearEnd { get; set; }
        public int MonthEnd { get; set; }

        public string[] RegionCodes { get; set; }
        public int[] TradeNameIds { get; set; }
        public int[] BrandIds { get; set; }
        public int[] ClassifierIds { get; set; }

        public DetailingType DetailingType { get; set; }
        public PeriodDetailingType PeriodDetailingType { get; set; }
    }
}