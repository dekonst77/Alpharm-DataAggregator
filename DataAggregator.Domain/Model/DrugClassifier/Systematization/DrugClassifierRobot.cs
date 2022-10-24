using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugClassifierRobot", Schema = "Systematization")]
    public class DrugClassifierRobot
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long DrugClearId { get; set; }

        public long? DrugId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        public int? ConsumerPackingCount { get; set; }
   
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? DateAdd { get; set; }

        public int Version { get; set; }

        public virtual DrugClear DrugClear { get; set; }
    }
}
