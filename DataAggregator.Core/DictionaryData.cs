using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Common;
using System.Collections.Generic;
using System.Linq;


namespace DataAggregator.Core
{
    public class DictionaryData
    {
        public static IEnumerable<DictionaryItem> GetData(DrugClassifierContext context, string dictionaryName, string value, int? specifiedCount)
        {
            int count = specifiedCount == null ? 10 : (int) specifiedCount;

            switch (dictionaryName)
            {
                case "tradeName":
                    return context.TradeNames.Where(d => d.Value.Contains(value)).OrderBy(d=>d.Value).Take(count).ToList();
                case "goodsTradeName":
                    return context.GoodsTradeName.Where(d => d.Value.Contains(value)).OrderBy(d => d.Value).Take(count).ToList();
                case "GoodsBrand":
                    return context.Brand.Where(d=>d.UseGoodsClassifier).Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem { Id = c.Id, Value = c.Value }).ToList();
                case "goodsDescription":
                    return context.Goods.Where(d => d.GoodsDescription.Contains(value)).Take(count).Select(c => new DictionaryItem { Id =c.Id, Value = c.GoodsDescription }).ToList();
                case "innGroup":
                    return context.INNGroups.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id =c.Id, Value = c.Description}).ToList();
                case "inn":
                    return context.INNs.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "packer":
                case "ownerTradeMark":
                case "Manufacturer":
                case "OwnerRegistrationCertificate":
                    return context.Manufacturer.Where(d => d.Value.Contains(value)).OrderBy(d => d.Value).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "Manufacturer_eng":
                    return context.Manufacturer.Where(d => d.Value_eng.Contains(value)).OrderBy(d => d.Value_eng).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value_eng }).ToList();
                case "Corporation":
                    return context.Corporation.Where(d => d.Value.Contains(value)).Take(count).ToList();
                case "Corporation_eng":
                    return context.Corporation.Where(d => d.Value_eng.Contains(value)).OrderBy(d => d.Value_eng).Take(count).ToList();
                //case "Country":
                //    return context.Country.Where(d => d.Value.Contains(value)).Take(count).ToList();
                case "formProduct":
                    return context.FormProducts.Where(d => d.Value.Contains(value)).Take(count).ToList();
                case "dosageGroup":
                    return context.DosageGroups.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Description }).ToList();
                case "dosage":
                    return context.Dosages.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "packing":
                    return context.Packings.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = (long)c.Id, Value = c.Value}).ToList(); 
                case "pack":
                    return context.Packings.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = (long)c.Id, Value = c.Value }).ToList();
                case "circulationPeriod":
                    return context.CirculationPeriod.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "brand":
                case "Brand":
                    return context.Brand.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem { Id = c.Id, Value = c.Value }).ToList();
                case "ATCBaa":
                    return context.ATCBaa.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Value, Description = c.Description}).ToList();
                case "ATCBaaDescription":
                    return context.ATCBaa.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Description }).ToList();
                case "ATCEphmra":
                    return context.ATCEphmra.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Value, Description = c.Description}).ToList();
                case "ATCEphmraDescription":
                    return context.ATCEphmra.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Description }).ToList();
                case "ATCWho":
                    return context.ATCWho.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Value, Description = c.Description}).ToList();
                case "ATCWhoDescription":
                    return context.ATCWho.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Description }).ToList();
                case "FTG":
                    return context.FTG.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem { Id = c.Id, Value = c.Value}).ToList();
                case "NFC":
                    return context.NFC.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Value, Description = c.Description }).ToList();
                case "NFCDescription":
                    return context.NFC.Where(d => d.Description.Contains(value)).Take(count).Select(c => new DictionaryDescriptionItem { Id = c.Id, Value = c.Description }).ToList();
                case "corporation":
                    return context.Corporation.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "drugType":
                    return context.DrugType.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "equipment":
                    return context.Equipment.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
                case "productionStage":
                    return context.ProductionStage.Where(d => d.Value.Contains(value)).Take(count).Select(c => new DictionaryItem() { Id = c.Id, Value = c.Value }).ToList();
            }

            return new DictionaryItem[0];
        }
    }
}
