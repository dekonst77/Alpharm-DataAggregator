using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    [Table("BlisterBlockView", Schema = "Classifier")]
    public class BlisterBlockView
    {
        [Key]
        public long ClassifierId { get; set; }
        public Nullable<decimal> OFD_Sum_LastMonth { get; set; }
        public Nullable<decimal> Audit_Sum_LastMonth { get; set; }
        public Nullable<int> ClassifierPackingId { get; set; }
        public Nullable<int> CountPrimaryPacking { get; set; }
        public Nullable<bool> IsExist { get; set; }
        public string Number { get; set; }
        public long DrugId { get; set; }
        public string Trade_Name { get; set; }
        public string DrugDescription { get; set; }
        public string INNGroup { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long PackerId { get; set; }
        public string Packer { get; set; }
        public bool Used { get; set; }
        public Nullable<bool> IsOther { get; set; }
        public string Comment { get; set; }
    }
}
