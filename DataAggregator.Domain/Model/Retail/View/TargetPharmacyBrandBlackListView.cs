using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class TargetPharmacyBrandBlackListView
    {
        [Key]
        [Column(Order = 1)]
        public long TargetPharmacyId { get; set; }

        [Key]
        [Column(Order = 2)]
        public long? BrandId { get; set; }

        public string Brand { get; set; }
    }
}
