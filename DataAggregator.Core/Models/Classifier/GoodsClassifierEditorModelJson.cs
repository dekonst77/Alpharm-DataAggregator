using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataAggregator.Domain.Model.Common;
using DataAggregator.Web.Models.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class GoodsClassifierEditorModelJson
    {
        public long GoodsId { get; set; }
        public string GoodsDescription { get; set; }
        public DictionaryJson GoodsTradeName { get; set; }
        public DictionaryJson GoodsBrand { get; set; }
        public DictionaryJson OwnerTradeMark { get; set; }
        public DictionaryJson Packer { get; set; }
        public long ProductionInfoId { get; set; }
        public GoodsCategoryJson GoodsCategory { get; set; }
        public long PackerId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string GoodKey { get; set; }
        public List<long> ParameterIds { get; set; }
        public bool Used { get; set; }
        public string Comment { get; set; }
        public bool ToRetail { get; set; }
        public void ClearAllStringProperties()
        {
            GoodsDescription = ClearString(GoodsDescription).Trim();
            GoodsTradeName.Value = ClearString(GoodsTradeName.Value).Trim();
            GoodsBrand.Value = ClearString(GoodsBrand.Value).Trim();
            OwnerTradeMark.Value = ClearString(OwnerTradeMark.Value).Trim();
            Packer.Value = ClearString(Packer.Value).Trim();
        }

        private string ClearString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            //Замена не разрывных пробелов на обычные
            value = Regex.Replace(value, @"\u00A0", " ");
            //Удаление двойных пробелов
            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ").Trim();
            }
            if (string.Equals(value, "~"))
                return null;

            return value;
        }
   }
}