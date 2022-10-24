using System;
using System.Collections;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class RealPacking
    {
        public long Id { get; set; }
        public int RealPackingCount { get; set; }
        public long DrugId { get; set; }

        public virtual Drug Drug { get; set; }

    }
}