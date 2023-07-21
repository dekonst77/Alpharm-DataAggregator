using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport
{
    [Table("RegCertificateNumberExceptions", Schema = "report")]
    public class RegCertificateNumberExceptions
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RegistrationCertificate")]
        public long RegistrationCertificateId { get; set; }
        public virtual RegistrationCertificate RegistrationCertificate { get; set; }

        [ForeignKey("ClassifierReport")]
        public byte ClassifierReportId { get; set; }

        public virtual ClassifierReport ClassifierReport { get; set; }
    }
}
