using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Bad
{
    [Table("BadData", Schema = "Bad")]
    public class BadData
    {
        public long Id { get; set; }
        public long Code { get; set; }
        public string RegNumber { get; set; }
        public string RegComp { get; set; }
        public long TypographNumber { get; set; }
        public string ProductDescription { get; set; }
        public string Manufacture { get; set; }
        public string Scope { get; set; }
        public Guid RawDataId { get; set; }
    }
}
