using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class Region
    {
        public long Id { get; set; }

        public string FederalDistrict { get; set; }

        public string FederationSubject { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public string Code { get; set; }
        public int Level { get; set; }
    }
    [Table("GS_Bricks_L3", Schema = "dbo")]
    public class GS_Bricks_L3
    {
        [Key]
        public string Id {  get;set;}
        public string Value { get; set; }
    }
}
