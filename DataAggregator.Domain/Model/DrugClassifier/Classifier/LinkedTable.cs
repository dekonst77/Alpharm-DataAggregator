using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class LinkedTable
    {
        public long Id { get; set; }

        public long DrugId { get; set; }

        public long Code { get; set; }

        public long HtmlId { get; set; }

        public Guid? HtmlOldId { get; set; }

        public string RegNum { get; set; }

        public bool? Multipage { get; set; }

        public bool? MultipageProdStage { get; set; }

        public long DrugInfoId { get; set; }

        public string Error { get; set; }
    }
}
