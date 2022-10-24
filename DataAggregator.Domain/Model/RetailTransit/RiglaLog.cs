using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.RetailTransit
{
    [Table("RiglaLog", Schema = "rigla")]
    public class RiglaLog
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public bool IsError { get; set; }
    }
}
