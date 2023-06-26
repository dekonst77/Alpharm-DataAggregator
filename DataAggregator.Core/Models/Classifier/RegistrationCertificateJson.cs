using System;
using System.Linq;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Core.Models.Classifier
{
    public class RegistrationCertificateJson
    {
        public long Id { get; set; }
        /// <summary>
        /// Дата регистрации
        /// </summary>
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? RegistrationDate { get; set; }
        /// <summary>
        /// Дата переоформления
        /// </summary>
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ReissueDate { get; set; }
        /// <summary>
        /// Срок введения в гражданский оборот
        /// </summary>
        public DictionaryJson CirculationPeriod { get; set; }
        /// <summary>
        /// Дата окончания действия
        /// </summary>
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ExpDate { get; set; }

        public string Number { get; set; }

        public DictionaryJson OwnerRegistrationCertificate { get; set; }

        //Исходный идентификатор
        public long? OwnerRegistrationCertificateId { get; set; }

        public bool IsBlocked { get; set; }

        //Срок годности
        public string StorageLife { get; set; }

        public RegistrationCertificateJson()
        {
            this.Id = 0;
            this.Number = null;
            this.CirculationPeriod = new DictionaryJson();
            this.ExpDate = null;
            this.ReissueDate = null;
            this.RegistrationDate = null;
            this.OwnerRegistrationCertificate = new DictionaryJson();
            this.OwnerRegistrationCertificateId = null;
            this.IsBlocked = true;

        }

        public RegistrationCertificateJson(RegistrationCertificate reg)
        {
            if (reg != null)
            {
                this.Id = reg.Id;
                this.Number = reg.Number;
                
                this.CirculationPeriod = new DictionaryJson(reg.CirculationPeriod);
                this.ExpDate = reg.ExpDate;
                this.RegistrationDate = reg.RegistrationDate;
                this.ReissueDate = reg.ReissueDate;
              
                this.OwnerRegistrationCertificate = new DictionaryJson(reg.OwnerRegistrationCertificate);
                this.OwnerRegistrationCertificateId = reg.OwnerRegistrationCertificateId;

                if(reg.OwnerRegistrationCertificate != null)
                {
                    this.OwnerRegistrationCertificate.Key = reg.OwnerRegistrationCertificate.Id.ToString();
                }

                this.IsBlocked = reg.IsBlocked;

                this.StorageLife = reg.RegistrationCertificateClassification.FirstOrDefault().StorageLife;
            }
        }

        public bool IsNull()
        {
            return !RegistrationDate.HasValue &&
                   !ReissueDate.HasValue &&
                   string.IsNullOrEmpty(CirculationPeriod.Value) &&
                   !ExpDate.HasValue &&
                   string.IsNullOrEmpty(Number) &&
                   string.IsNullOrEmpty(OwnerRegistrationCertificate.Value);
        }
    }
}