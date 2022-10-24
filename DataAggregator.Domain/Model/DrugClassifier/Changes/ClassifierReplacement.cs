using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Domain.Model.DrugClassifier.Changes
{

    [Table("ClassifierReplacement", Schema = "changes")]
    public class ClassifierReplacement
    {
        public long Id { get; set; }

        public long ClassifierIdFrom { get; set; }

        public long ClassifierIdTo { get; set; }

        public string DescriptionFrom { get; set; }

        public string DescriptionTo { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
      
    }
}
