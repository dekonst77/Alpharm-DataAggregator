using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class FileInfoLog
    {
        public long Id { get; set; }

        public long FileInfoId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public string Text { get; set; }

        public string StackTrace { get; set; }

        public bool IsError { get; set; }

        public virtual FileInfo FileInfo { get; set; }

        public string ErrorMessage { get; set; }
    }
}
