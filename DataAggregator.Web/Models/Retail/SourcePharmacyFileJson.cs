using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail
{
    public class SourcePharmacyFileJson
    {
        public long Id { get; set; }

        public long SourcePharmacyId { get; set; }

        public string FileName { get; set; }
    }
}