using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class SourcePharmacyGroup
    {
        public long Id { get; set; }

        [JsonIgnore]
        public virtual List<SourcePharmacy> SourcePharmacy { get; set; }

        public virtual List<SourcePharmacyGroupFile> SourcePharmacyGroupFile { get; set; }

        public string GroupName { get; set; }

        public string FileNames { get; set; }

        public long? SourceId { get; set; }

        public virtual Source Source { get; set; }

        public bool IsWithSource { get { return Source != null; } }
    }
}
