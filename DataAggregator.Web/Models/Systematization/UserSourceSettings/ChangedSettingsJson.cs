using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Systematization.UserSourceSettings
{
    public class ChangedSettingsJson
    {
        public string UserId { get; set; }
        public long NewPeriodId { get; set; }
        public long OldDbPeriodId { get; set; }

        public long NewDbPeriodId { get; set; }
        public string LastEditorName { get; set; }
    }
}