namespace DataAggregator.Web.Models.Systematization
{
    public class SynonymJson
    {
        public long Id { get; set; }
        public long DrugClearId { get; set; }
        public long OriginalId { get; set; }
        public string SynTableName { get; set; }
        public string Value { get; set; }
    }
}