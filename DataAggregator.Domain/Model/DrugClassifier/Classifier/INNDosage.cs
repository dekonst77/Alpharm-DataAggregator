namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class INNDosage
    {
        public long Id { get; set; }
        public string DosageCount { get; set; }
        public long? DosageId { get; set; }
        public virtual Dosage Dosage { get; set; }
        public long? DosageGroupId { get; set; }
        public virtual DosageGroup DosageGroup { get; set; }
        public long? Order { get; set; }
    }
}
