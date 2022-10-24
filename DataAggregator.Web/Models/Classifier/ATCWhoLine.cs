namespace DataAggregator.Web.Models.Classifier
{
    public class AtcWhoLine
    {
        public long Atc1Id { get; set; }
        public long? Atc2Id { get; set; }
        public long? Atc3Id { get; set; }
        public long? Atc4Id { get; set; }
        public long? Atc5Id { get; set; }

        public string Atc1Value { get; set; }
        public string Atc2Value { get; set; }
        public string Atc3Value { get; set; }
        public string Atc4Value { get; set; }
        public string Atc5Value { get; set; }

        public string Atc1Description { get; set; }
        public string Atc2Description { get; set; }
        public string Atc3Description { get; set; }
        public string Atc4Description { get; set; }
        public string Atc5Description { get; set; }
    }
}