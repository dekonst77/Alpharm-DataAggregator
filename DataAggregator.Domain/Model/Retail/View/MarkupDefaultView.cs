using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.View
{
    [Table("MarkupDefaultView", Schema = "dbo")]
    public class MarkupDefaultView
    {
        [Key]
        public long Id { get; set; }

        public int PriceMin { get; set; }

        public int? PriceMax { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public decimal Markup { get; set; }
    }
}
