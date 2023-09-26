using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class OrphanView
    {
        [Key]
        public int Id { get; set; }
        public long INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public Nullable<long> FormProductId { get; set; }
        public string FormProduct { get; set; }
        public Nullable<long> DosageGroupId { get; set; }
        public string DosageGroup { get; set; }
        public bool IsOrphan { get; set; }
        public bool InDecreeRussianGovernment { get; set; }
        public bool InListHealthMinistry { get; set; }
        public bool InGRLS { get; set; }
        public bool InWithoutReg { get; set; }
    }
}
