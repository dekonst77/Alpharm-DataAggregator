namespace DataAggregator.Core.Models.Classifier
{
    public class INNGroupDosageJson
    {
        public DictionaryJson INN { get; set; }

        public string DosageCount { get; set; }

        public DictionaryJson Dosage { get; set; }

        public INNGroupDosageJson()
        {
            
        }
    }
}