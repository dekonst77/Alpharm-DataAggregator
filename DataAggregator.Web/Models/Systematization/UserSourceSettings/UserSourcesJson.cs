using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Systematization.UserSourceSettings
{
    public class UserSourceJson
    {
        public Guid UserId { get; set; }

        public string UserFullName { get; set; }

        public string DepartmentShortName { get; set; }

        public long PeriodId { get; set; } 
    }
}