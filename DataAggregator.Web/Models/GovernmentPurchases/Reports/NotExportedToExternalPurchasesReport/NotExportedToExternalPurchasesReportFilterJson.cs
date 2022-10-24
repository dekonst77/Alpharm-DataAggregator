using System;

namespace DataAggregator.Web.Models.GovernmentPurchases.Reports.NotExportedToExternalPurchasesReport
{
    public class NotExportedToExternalPurchasesReportFilterJson
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string ReportObject { get; set; }
    }
}