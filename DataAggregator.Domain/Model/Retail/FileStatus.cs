using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class FileStatus
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual IList<FileInfo> FileInfo { get; set; } 
    }
}
