using DataAggregator.Domain.Model.Retail.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail.Report
{
    public class ReportLauncherModel
    {
        public long Id { get; set; }
        public long ReportId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int StatusId { get; set; }
        public long UserId { get; set; }
        public string ReportName { get; set; }
        public string UserFullName { get; set; }
        public string StatusName { get; set; }
        public DateTime Date { get; set; }
        public bool SendSelf { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Email { get; set; }
        public static ReportLauncherModel Create(ReportLauncher view)
        {
            return ModelMapper.Mapper.Map<ReportLauncherModel>(view);
        }
    }
}