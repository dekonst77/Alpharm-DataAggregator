using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.LPU
{
    [Table("LPUType", Schema = "dbo")]
    public class LPUType
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [Table("LPUKind", Schema = "dbo")]
    public class LPUKind
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
