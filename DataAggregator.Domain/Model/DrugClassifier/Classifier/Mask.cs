using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class Mask
    {
        public int Id { get; set; }

        public long FromClassifierId { get; set; }
        public long ToClassifierId { get; set; }

        public DateTime? DateInsert { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid? UserId { get; set; }
        public bool Manual { get; set; }
        public bool Use { get; set; }

        public Guid? UserReplaceId { get; set; }
        
    }
}
