using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class AutomaticProcessesLogView
    {
        [Key]
        public Guid TaskGuid { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public string Arguments { get; set; }
        public string Result { get; set; }
    }
}
