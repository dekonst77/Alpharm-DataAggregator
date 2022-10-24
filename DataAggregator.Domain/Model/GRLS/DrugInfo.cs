using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GRLS
{
    //[Table("DrugInfo", Schema = "dbo")]
    public class DrugInfo
    {
        public long Id { get; set; }
        public string Narcotic { get; set; }
        public string VED { get; set; }
        public string FTG { get; set; }
        public string Barcode { get; set; }
        public string INN { get; set; }
        public string TradeName { get; set; }
        public long RegistrationCertificateId { get; set; }
        public long PageId { get; set; }
        public string ATXCode { get; set; }
        public string ATXDescription { get; set; }
        public bool ManyProductionStagePage { get; set; }


        public virtual RegistrationCertificate RegistrationCertificate { get; set; }
        public virtual IList<ProductionStage> ProductionStage { get; set; } 
    }
}