using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class Source
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public virtual IList<Template> Template { get; set; }

        [JsonIgnore]
        public virtual IList<FileInfo> FileInfo { get; set; } 
    }
}
