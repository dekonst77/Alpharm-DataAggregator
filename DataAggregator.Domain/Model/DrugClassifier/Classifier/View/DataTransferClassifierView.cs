using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class DataTransferClassifierView
    {
        [Key]
        public long ClassifierId { get; set; }

        public long DrugId { get; set; }

        public long PackerId { get; set; }

        public long OwnerTradeMarkId { get; set; }


        public string TradeName { get; set; }

        public string DrugDescription { get; set; }

        public string Packer { get; set; }

        public string OwnerTradeMark { get; set; }

        public bool IsHistorical { get; set; }
    }
}
