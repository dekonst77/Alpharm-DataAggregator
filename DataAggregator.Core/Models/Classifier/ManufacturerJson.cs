using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Core.Models.Classifier
{
    public class ManufacturerJson
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Value_eng { get; set; }
        public long? CorporationId { get; set; }
        public string Corporation_Value { get; set; }
        public string Corporation_Value_eng { get; set; }


        public long? CountryId { get; set; }
        public DictionaryJson Country { get; set; }

        public string filter { get; set; }
    }
}
