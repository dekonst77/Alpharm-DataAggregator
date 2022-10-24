using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class RegistrationCertificateView
    {
        [Key]
        public Guid Guid { get; set; }

        public long DrugId { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public long? RegistrationCertificateId { get; set; } 
    }
}