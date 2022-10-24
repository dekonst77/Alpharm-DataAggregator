using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class MaskView
    {
        public int Id { get; set; }
        public long FromClassifierId { get; set; }
        public long ToClassifierId { get; set; }
        public DateTime? DateInsert { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string User { get; set; }
        public bool Manual { get; set; }
        public bool Block { get; set; }
        public int FromDrugId { get; set; }
        public string FromTradeName{ get; set; }
        public int FromOwnerTradeMarkId { get; set; }
        public string FromDrugDescription{ get; set; }
        public string FromOwnerTradeMark{ get; set; }
        public int FromPackerId { get; set; }
        public string FromPacker{ get; set; }
        public int ToDrugId { get; set; }
        public string ToTradeName{ get; set; }
        public int ToOwnerTradeMarkId { get; set; }
        public string ToDrugDescription{ get; set; }
        public string ToOwnerTradeMark{ get; set; }
        public int ToPackerId { get; set; }
        public string ToPacker{ get; set; }
        public string ReplaceUser { get; set; }


    }
}
