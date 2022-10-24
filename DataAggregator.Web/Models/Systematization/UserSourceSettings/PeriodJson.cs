using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;

namespace DataAggregator.Web.Models.Systematization.UserSourceSettings
{
    public class PeriodJson
    {
        public PeriodJson(long periodId, string periodName, long periodSourceId, string periodSourceName, SourceStat sourceStat)
        {
            Id = periodId;
            DisplayName = periodName;
            SourceId = periodSourceId;
            SourceName = periodSourceName;
            
            ForCheckingCount = (sourceStat != null) ? sourceStat.ForCheckingCount : null;
            ForAddingCount = (sourceStat != null) ? sourceStat.ForAddingCount : null;
            WorkCount = (sourceStat != null) ? sourceStat.WorkCount : null;
            WorkCount_Dop = (sourceStat != null) ? sourceStat.WorkCount_Dop : null;
        }

        public long Id { get; set; }
        public string DisplayName { get; set; }
        public long SourceId { get; set; }
        public string SourceName { get; set; }

        public long? ForCheckingCount { get; set; }
        public long? ForAddingCount { get; set; }
        public long? WorkCount { get; set; }
        public long? WorkCount_Dop { get; set; }
    }
}