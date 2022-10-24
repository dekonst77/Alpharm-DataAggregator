using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.Reports.WrongPricesReport
{
    public class WrongPricesReportFilterJson
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IncludePurchases { get; set; }
        public bool IncludeContracts { get; set; }
        public decimal LessCoeff { get; set; }
        public decimal MoreCoeff { get; set; }
    }
}