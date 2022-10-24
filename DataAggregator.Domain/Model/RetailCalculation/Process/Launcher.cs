using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("Launcher", Schema = "process")]
    public class Launcher
    {
        public int Id { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public  int ProcessId { get; set; }

        public Guid? UserId { get; set; }

        public int StatusId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Comment { get; set; }

        public  virtual  Process Process { get; set; }

        public virtual Status Status { get; set; }

        public virtual  User User { get; set; }
    }
}
