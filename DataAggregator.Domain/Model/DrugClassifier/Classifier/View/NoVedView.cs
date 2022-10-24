using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class NoVedView
    {
        [Key, Column(Order = 1)]
        public long INNGroupId { get; set; }
        [Key, Column(Order = 2)]
        public long FormProductId { get; set; }
        public DateTime? DateAdd { get; set; }

        public bool Checked { get; set; }

        public virtual INNGroup InnGroup { get; set; }
        public virtual FormProduct FormProduct { get; set; }
    }
}