using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.dict
{
    [Table("SupplierResultOnewWinner", Schema = "dict")]
    public class SupplierResultOnewWinner
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
