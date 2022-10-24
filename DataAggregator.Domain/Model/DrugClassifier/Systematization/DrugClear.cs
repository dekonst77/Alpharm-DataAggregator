using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.DAL;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugClear", Schema = "Systematization")]
    public class DrugClear
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public string Manufacturer { get; set; }

        public DateTime Date { get; set; }

        public long SourceId { get; set; }

        public string ShortText { get; set; }

        public virtual IList<DrugRaw> DrugRaw { get; set; }

        public virtual Source Source { get; set; }

        public virtual IList<DrugClearPeriod> DrugClearPeriod { get; set; }
        
        public virtual IList<DrugClassifierRobot> DrugClassifierRobot { get; set; }
    }
}
