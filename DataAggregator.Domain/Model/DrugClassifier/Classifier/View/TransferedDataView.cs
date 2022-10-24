using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class TransferedDataView
    {
        public long Id { get; set; }
        public long DrugIdFrom { get; set; }
        public string TradeNameFrom { get; set; }
        public string DrugDescriptionFrom { get; set; }
        public long PackerIdFrom { get; set; }
        public string PackerFrom { get; set; }
        public long OwnerTradeMarkIdFrom { get; set; }
        public string OwnerTradeMarkFrom { get; set; }
        public long DrugIdTo { get; set; }
        public string TradeNameTo { get; set; }
        public string DrugDescriptionTo { get; set; }
        public long PackerIdTo { get; set; }
        public string PackerTo { get; set; }
        public long OwnerTradeMarkIdTo { get; set; }
        public string OwnerTradeMarkTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
