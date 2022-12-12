using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport
{
    [Table("RegCertificateNumberExceptions", Schema = "report")]
    public class RegCertificateNumberExceptions
    {
        public int Id { get; set; }
        public Nullable<long> RegistrationCertificateId { get; set; }
        public Nullable<byte> ClassifierReportId { get; set; }

        public virtual ClassifierReport ClassifierReport { get; set; }
    }
}
