using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("FieldHtmlFieldOrhanization", Schema = "org")]
    public class FieldHtmlFieldOrhanization
    {
        public string Header { get; set; }
        public string FiledHtml { get; set; }
        public string FiledOrganization { get; set; }
        public long Id { get; set; }
        public bool Is223 { get; set; }
    }
}