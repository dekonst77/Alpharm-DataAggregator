using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class DrugInfoJson
    {
        public long Id { get; set; }

        public DictionaryJson TradeName { get; set; }

        public InnGroupJson INNGroup { get; set; }

        public List<DrugJson> Drug { get; set; } 

        public DrugInfoJson(Drug drug)
        {
            var drugInfo = drug.DrugInfo;

            Id = drugInfo.Id;
            TradeName =  new DictionaryJson(drugInfo.TradeName);
            INNGroup = new InnGroupJson(drugInfo.INNGroup);
            SelectedDrugId = drug.Id;
            Drug = drugInfo.Drugs.Select(d => new DrugJson(d)).ToList();
        }

        public DrugInfoJson(DrugInfo drugInfo)
        {
            Id = drugInfo.Id;
            TradeName = new DictionaryJson(drugInfo.TradeName);
            INNGroup = new InnGroupJson(drugInfo.INNGroup);
            Drug = drugInfo.Drugs.Select(d => new DrugJson(d)).ToList();
        }

        public long SelectedDrugId { get; set; }

    }
}