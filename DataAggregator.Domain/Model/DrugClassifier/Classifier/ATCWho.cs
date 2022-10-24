

using DataAggregator.Domain.Model.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class ATCWho : HierarchicalDictionaryItem<ATCWho>
    {
        public bool IsUse { get; set; }
    }
    [Table("MKB", Schema = "Classifier")]
    public class MKB
    {
        [Key]
        public int id { get; set; }
        public string mkb_code { get; set; }
        public string mkb_name { get; set; }
    }

    [Table("ATCWhoLinkMKBView", Schema = "Classifier")]
    public class ATCWhoLinkMKBView
    {
        [Key]
        [Column(Order = 1)]
        public int ATCWhoId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int? MKBId { get; set; }
        public string ATCWho_Value { get; set; }
        public string ATCWho_Description { get; set; }
        public string mkb_code { get; set; }
        public string mkb_name { get; set; }
    }
}