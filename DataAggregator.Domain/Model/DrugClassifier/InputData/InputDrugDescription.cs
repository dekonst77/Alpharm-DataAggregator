using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.InputData
{

    [Table("InputDrugDescription", Schema = "InputData")]
    public class InputDrugDescription 
    {
        public long Id { get; set; }

        public string Generic { get; set; }

        public string Dosage { get; set; }

        public string FormProduct { get; set; }

        public string Packing { get; set; }

        public string TradeName { get; set; }

        public string TradeMark { get; set; }

        public string DosageFormProductPacking { get; set; }

        public long InputDataSourceId { get; set; }

        public string Barcodes { get; set; }

        public string RegistrationCertificate { get; set; }

        public virtual InputDataSource InputDataSource { get; set; }
    }
}
