using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class FileInfoView
    {
        public long Id { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public long FileStatusId { get; set; }
        public string FileStatus { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
    }
}
