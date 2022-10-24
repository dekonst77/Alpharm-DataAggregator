using DataAggregator.Domain.Model.Retail.View;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.Report
{
    [Table("Launcher", Schema = "report")]
    public class ReportLauncher
    {
        public long Id { get; set; }
        public long ReportId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long StatusId { get; set; }
        public string Email { get; set; }

        public DateTime? DateEnd { get; set; }
        public string ErrorMessage { get; set; }
       
        public Guid UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public virtual Report Report { get; set; }

        public virtual Status Status { get; set; }

      
        public virtual User User { get; set; }

    }
}
