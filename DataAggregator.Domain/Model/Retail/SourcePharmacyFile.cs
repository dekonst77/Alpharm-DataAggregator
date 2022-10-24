using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class SourcePharmacyFile
    {
        public long Id { get; set; }

        public long SourcePharmacyId { get; set; }

        [JsonIgnore]
        public virtual SourcePharmacy SourcePharmacy { get; set; }

        public string FileName { get; set; }
    }
}
