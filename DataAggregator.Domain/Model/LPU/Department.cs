using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.LPU
{
    [Table("Department", Schema = "lpu")]
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("LPU_Department", Schema = "dbo")]
    public class LPU_Departments
    {
        public long Id { get; set; }
        public long LPUId_Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
  
    }
    
}
