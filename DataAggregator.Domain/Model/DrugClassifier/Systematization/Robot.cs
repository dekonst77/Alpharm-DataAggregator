using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("Robot", Schema = "Systematization")]
    public class Robot
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<DrugClassifier> DrugClassifier { get; set; }

        //public virtual IList<StatusHistory> StatusHistory { get; set; } 
    }
}
