using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier.AddingDOPMonitoringDatabase
{
    public class GetBlocking_Result
    {
        [Key]
        public long BlockingForMonitoringId { get; set; }
        public long GoodsCategoryId { get; set; }
        public Nullable<long> ParameterId { get; set; }
        public Nullable<long> ClassifierId { get; set; }
        public Nullable<bool> Status { get; set; }
        public string StatusDesc { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public int BlockTypeId { get; set; }
        public string BlockTypeName { get; set; }
        public string BlockTypeDescription { get; set; }
        public string GoodsCategoryName { get; set; }
        public string ParameterValue { get; set; }
    }
}
