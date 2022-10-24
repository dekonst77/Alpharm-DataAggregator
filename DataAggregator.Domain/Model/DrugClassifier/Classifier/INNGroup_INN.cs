using System.Collections;
using System.Collections.Generic;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class INNGroup_INN
    {
        public long Id { get; set; }
        public long INNGroupId { get; set; }
        public long INNId { get; set; }
        public long Order { get; set; }

        public virtual INN INN { get; set; }
        public virtual INNGroup InnGroup { get; set; }
    }
}