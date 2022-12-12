using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport
{
    /// <summary>
    /// 1 Лист - список исключений
    /// </summary>
    public class ClassifierCheckReportExceptionListResult
    {
        public int Id { get; set; }
        public Nullable<long> RegistrationCertificateId { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public Nullable<byte> ClassifierReportId { get; set; }
        public string ReportCode { get; set; }
    }

    /// <summary>
    /// 2 лист - TN+Brand
    /// </summary>
    public class ClassifierCheckReportTNBrandListResult
    {
        public string RegistrationCertificateNumber { get; set; }
        public string TradeName { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// 3 лист - ATCWhoDescription
    /// </summary>
    public class ClassifierCheckReportATCWhoDescriptionListResult
    {
        public string RegistrationCertificateNumber { get; set; }
        public string ATCWhoCode { get; set; }
        public string ATCWhoDescription { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// 4 лист - FTG
    /// </summary>
    public class ClassifierCheckReportFTGListResult
    {
        public string RegistrationCertificateNumber { get; set; }
        public long FTGId { get; set; }
        public string FTG { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// 5 лист - ATCEphmraDescription
    /// </summary>
    public class ClassifierCheckReportATCEphmraDescriptionListResult
    {
        public string RegistrationCertificateNumber { get; set; }
        public string ATCEphmraCode { get; set; }
        public string ATCEphmraDescription { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// 6 лист - FormProduct
    /// </summary>
    public class ClassifierCheckReportFormProductListResult
    {
        public string RegistrationCertificateNumber { get; set; }
        public long FormProductId { get; set; }
        public string FormProduct { get; set; }
        public int Count { get; set; }
    }
}
