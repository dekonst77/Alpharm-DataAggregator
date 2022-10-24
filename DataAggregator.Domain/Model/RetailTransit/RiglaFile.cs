using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.RetailTransit
{
    [Table("RiglaFile", Schema = "rigla")]
    public class RiglaFile
    {
        [Key]
        public string FileName { get; set; }
    }
}
