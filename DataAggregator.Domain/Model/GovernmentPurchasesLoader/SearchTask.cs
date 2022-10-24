using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("SearchTask", Schema = "search")]
    public class SearchTask
    {
        public SearchTask()
        {
            this.SearchPage = new HashSet<SearchPage>();
        }

        public long Id { get; set; }
        public string Law { get; set; }
        public System.DateTime DatePublishStart { get; set; }
        public System.DateTime DatePublishEnd { get; set; }
        public string WithSubElements { get; set; }
        public string Okpd { get; set; }
        public string Okpd2 { get; set; }
        public string StagePurchase { get; set; }
        public string SearchString { get; set; }
        public int NextPage { get; set; }
        public bool IsLoaded { get; set; }
        public System.DateTime DateAdd { get; set; }
        public System.DateTime? LastTryLoad { get; set; }
        public string Okved { get; set; }
        public string Okved2 { get; set; }
        public string Okdp { get; set; }
        public string SearchUrl { get; set; }

        public virtual ICollection<SearchPage> SearchPage { get; set; }
        public int? StatusId { get; set; }
        public string ErrorMessage { get; set; }
        public int FoundLinkCount { get; set; }
    }
}