using System.Collections.Generic;
using System.Text;

namespace DataAggregator.Core.Filter
{
    /// <summary>
    /// Дополнительный фильтр
    /// </summary>
    public class AdditionalGoodsFilter
    {
        public long? GoodsId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        public string Text { get; set; }

        public string Goods { get; set; }

        public string Manufacturer { get; set; }

        public string Packer { get; set; }

        public List<long> DrugClearId { get; set; }
        
        public AdditionalGoodsFilter()
        {
            DrugClearId = new List<long>();
        }

        public string GetFilter()
        {
            var mainCondition = new StringBuilder();
            var subConditions = new List<string>();

            if (DrugClearId.Count > 0)
            {
                StringBuilder dciCondition = new StringBuilder();
                foreach (var dci in DrugClearId)
                {
                    if (dciCondition.Length > 0)
                    {
                        dciCondition.Append(" or ");
                    }
                    dciCondition.Append(string.Format("gc.DrugClearId = {0}", dci));
                }
                subConditions.Add(dciCondition.ToString());
            }

            if (GoodsId.HasValue)
                subConditions.Add(string.Format("c.GoodsId = {0}", GoodsId));

            if (OwnerTradeMarkId>0)
                subConditions.Add(string.Format("c.OwnerTradeMarkId in (SELECT Id FROM Classifier.Manufacturer WHERE [Id] = '{0}')", OwnerTradeMarkId));

            if (PackerId>0)
                subConditions.Add(string.Format("c.PackerId in (SELECT t.Id FROM Classifier.Manufacturer t WHERE t.[Id] = '{0}')", PackerId));

            if (!string.IsNullOrWhiteSpace(Text))
                subConditions.Add(string.Format("dc.ShortText like '{0}'", Text.Replace("*", "%")));

            if (!string.IsNullOrWhiteSpace(Goods))
                subConditions.Add(string.Format("c.GoodsId in (SELECT t.Id FROM GoodsClassifier.Goods t WHERE t.GoodsDescription like '{0}')", Goods.Replace("*", "%")));

            if (!string.IsNullOrEmpty(Manufacturer))
                subConditions.Add(string.Format("dc.Manufacturer like '{0}'", Manufacturer.Replace("*", "%")));

            if (!string.IsNullOrWhiteSpace(Packer))
                subConditions.Add(string.Format("c.PackerId in (SELECT t.Id FROM Classifier.Manufacturer t WHERE t.Value like '{0}')", Packer.Replace("*", "%")));

            if (subConditions.Count > 0)
            {
                foreach (string subCondition in subConditions)
                {
                    if (mainCondition.Length > 0)
                        mainCondition.Append(" AND ");

                    mainCondition.Append(subCondition);
                }

                mainCondition.Insert(0, "(");
                mainCondition.Append(")");
            }

            return mainCondition.ToString();
        }
    }
}