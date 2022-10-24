using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.Retail;

namespace DataAggregator.Web.Models.Retail
{
    public class SourcePharmacyGroupJson
    {
        public SourcePharmacyGroupJson()
        {
            
        }

        public SourcePharmacyGroupJson(SourcePharmacyGroup sourcePharmacyGroup)
        {
            Id = sourcePharmacyGroup.Id;
            GroupName = sourcePharmacyGroup.GroupName;
            FileNames = sourcePharmacyGroup.FileNames;
            Source = sourcePharmacyGroup.Source;
            IsWithSource = sourcePharmacyGroup.IsWithSource;
        }

        public long Id { get; set; }

        public string GroupName { get; set; }

        public string FileNames { get; set; }

        public Source Source { get; set; }

        public bool IsWithSource { get; set; }
    }
}