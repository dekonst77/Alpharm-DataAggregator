using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization.View
{
    [Table("User", Schema = "Systematization")]
    public class User
    {
        public string FullName { get; set; }

        public string Id { get; set; }

        public string DepartmentShortName { get; set; }
    }
}
