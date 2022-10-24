using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System;

namespace DataAggregator.Web.Models.Classifier
{
    public class RegistrationCertificateModel
    {

        public long Id { get; set; }
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime? RegistrationDate { get; set; }
        /// <summary>
        /// Дата переоформления
        /// </summary>
        public DateTime? ReissueDate { get; set; }
        /// <summary>
        /// Срок введения в гражданский оборот
        /// </summary>
        public long? CirculationPeriodId { get; set; }
        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public DateTime? ExpDate { get; set; }

        public string Number { get; set; }

        public long? OwnerRegistrationCertificateId { get; set; }

        public bool IsBlocked { get; set; }
        
        public virtual Manufacturer OwnerRegistrationCertificate { get; set; }

        public virtual CirculationPeriod CirculationPeriod { get; set; }
    }
}