using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.Dadata
{
    [Table("SourceString", Schema = "dadata")]
    public class SourceString
    {
        public long Id { get; set; }

        public string String { get; set; }

        public bool IsLoaded { get; set; }
    }
}
