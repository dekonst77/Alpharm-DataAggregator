using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class SourcePharmacy
    {
        public long Id { get; set; }

        public bool IsSingle { get; set; }

        public string SourceName { get; set; }

        public string SourceNameDetailed { get; set; }

        public string EntityName { get; set; }

        public string PharmacyName { get; set; }

        public string PharmacyNumber { get; set; }

        public string NetName { get; set; }

        public string Address { get; set; }

        public string FiasGuid { get; set; }

        public string FileName { get; set; }

        public string FileName2 { get; set; }

        public long? TargetPharmacyId { get; set; }

        public string FileNames { get; set; }

        public bool Use { get; set; }

        public virtual IList<SourcePharmacyFile> SourcePharmacyFile { get; set; }

        public long? SourcePharmacyGroupId { get; set; }

        public virtual SourcePharmacyGroup SourcePharmacyGroup { get; set; }
    }
}
