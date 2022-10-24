using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.OkpdLoader
{
    [Table("LoadedOKPD", Schema = "dbo")]
    public class LoadedOKPD
    {
        public long Id { get; set; }
        public long Key { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
