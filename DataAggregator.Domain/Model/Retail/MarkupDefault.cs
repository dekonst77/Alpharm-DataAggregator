using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("MarkupDefault", Schema = "dbo")]
    public class MarkupDefault
    {
        [Key]
        public long Id { get; set; }

        public long MarkupPriceRangeId { get; set; }

        public string RegionPM1 { get; set; }

        public decimal Markup { get; set; }
    }
}
