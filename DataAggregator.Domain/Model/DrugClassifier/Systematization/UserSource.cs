using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("UserSource", Schema = "Systematization")]
    public class UserSource
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long SourceId { get; set; }

        public virtual Source Source { get; set; }
        
        public virtual long PeriodId { get; set; }

        public virtual Period Period { get; set; }

        public Guid? LastEditorId { get; set; }
    }
}
