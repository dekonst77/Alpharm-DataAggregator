using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail
{
    public class FieldsToSaveJson
    {
        public bool IsSingle { get; set; }

        public bool SourceName { get; set; }

        public bool SourceNameDetailed { get; set; }

        public bool EntityName { get; set; }

        public bool PharmacyName { get; set; }

        public bool PharmacyNumber { get; set; }

        public bool NetName { get; set; }

        public bool Address { get; set; }

        public bool FiasGuid { get; set; }

        public bool FileName { get; set; }

        public bool FileName2 { get; set; }

        public bool FileNames { get; set; }

        public bool TargetPharmacyId { get; set; }

        public bool SourcePharmacyGroup { get; set; }

        public bool Use { get; set; }
    }
}