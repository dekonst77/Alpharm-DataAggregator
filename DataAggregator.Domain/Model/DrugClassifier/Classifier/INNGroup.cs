﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class INNGroup
    {
        [Key]
        public long Id { get; set; }
        public string Description { get; set; }
        public bool IsCompound { get; set; }
        public bool IsCompoundBAA { get; set; }
        public bool IsException { get; set; }

        [JsonIgnore]
        public virtual ICollection<Drug> Drug { get; set; }

        [JsonIgnore]
        public virtual ICollection<INNGroup_INN> INNGroup_INN { get; set; }

        [JsonIgnore]
        public virtual ICollection<SynINNGroup> SynINNGroups { get; set; }

    }
}
