using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class SourcePharmacyGroupFile
    {
        public long Id { get; set; }

        public long SourcePharmacyGroupId { get; set; }

        [JsonIgnore]
        public virtual SourcePharmacyGroup SourcePharmacyGroup { get; set; }

        public string FileName { get; set; }
    }
}
