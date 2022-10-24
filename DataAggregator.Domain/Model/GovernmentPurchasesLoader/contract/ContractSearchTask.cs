using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.contract
{
    [Table("SearchTask", Schema = "contract")]
    public class ContractSearchTask
    {
        public long Id { get; set; }       
        public DateTime DatePublishStart { get; set; }
        public DateTime? DatePublishEnd { get; set; }   
        public string Okpd2 { get; set; }       
        public string SearchString { get; set; }
        public int NextPage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsLoaded { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime? LastTryLoad { get; set; }
        public string SearchUrl { get; set; }
        public int? StatusId { get; set; }
    }
}
