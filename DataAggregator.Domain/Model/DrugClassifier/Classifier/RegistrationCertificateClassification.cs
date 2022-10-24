using System;
using System.Collections.Generic;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class RegistrationCertificateClassification
    {
      
        public long Id { get; set; }
    
        public long RegistrationCertificateId { get; set; }
  
        public bool Exchangeable { get; set; }
       
        public bool Reference { get; set; }
    
        public string StorageLife { get; set; }

        public virtual RegistrationCertificate RegistrationCertificate { get; set; }


    }
}