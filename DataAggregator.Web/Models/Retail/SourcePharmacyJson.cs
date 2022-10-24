using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.Retail;

namespace DataAggregator.Web.Models.Retail
{
    public class SourcePharmacyJson
    {
        public SourcePharmacyJson()
        {
            
        }

        public SourcePharmacyJson(SourcePharmacy sourcePharmacy)
        {
            Id = sourcePharmacy.Id;
            IsSingle = sourcePharmacy.IsSingle;
            SourceName = sourcePharmacy.SourceName;
            SourceNameDetailed = sourcePharmacy.SourceNameDetailed;
            EntityName = sourcePharmacy.EntityName;
            PharmacyName = sourcePharmacy.PharmacyName;
            PharmacyNumber = sourcePharmacy.PharmacyNumber;
            NetName = sourcePharmacy.NetName;
            Address = sourcePharmacy.Address;
            FiasGuid = sourcePharmacy.FiasGuid;
            FileName = sourcePharmacy.FileName;
            FileName2 = sourcePharmacy.FileName2;
            TargetPharmacyId = sourcePharmacy.TargetPharmacyId;
            FileNames = sourcePharmacy.FileNames;
            SourcePharmacyGroupId = sourcePharmacy.SourcePharmacyGroupId;
            Use = sourcePharmacy.Use;
            SourcePharmacyGroup = sourcePharmacy.SourcePharmacyGroup == null
                ? "Нет группы"
                : sourcePharmacy.SourcePharmacyGroup.GroupName;
        }

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

        public long? SourcePharmacyGroupId { get; set; }

        public string FileNames { get; set; }

        public string SourcePharmacyGroup { get; set; }

        public bool Use { get; set; }
    }
}
