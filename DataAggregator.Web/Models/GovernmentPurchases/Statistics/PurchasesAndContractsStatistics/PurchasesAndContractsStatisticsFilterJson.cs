using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.Statistics.PurchasesAndContractsStatistics
{
    public class PurchasesAndContractsStatisticsFilterJson
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string StatisticsObject { get; set; }
    }
}