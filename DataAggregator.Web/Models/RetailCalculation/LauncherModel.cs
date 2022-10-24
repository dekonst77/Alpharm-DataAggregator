using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.RetailCalculation;
using DataAggregator.Web.Models.Retail.CountRuleEditor;

namespace DataAggregator.Web.Models.RetailCalculation
{
    public class LauncherModel
    {
        public int Id { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public int ProcessId { get; set; }

        public string Process { get; set; }

        public string User { get; set; }

        public string Status { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Comment { get; set; }

        public static LauncherModel Create(Launcher model)
        {
            return ModelMapper.Mapper.Map<LauncherModel>(model);
        }


    }
}