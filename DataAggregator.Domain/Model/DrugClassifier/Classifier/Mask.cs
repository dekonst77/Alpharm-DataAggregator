using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class Mask
    {
        public int Id { get; set; }
      
        public bool Use { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
