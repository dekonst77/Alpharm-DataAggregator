using System.Collections.Generic;
using System.Text;

namespace DataAggregator.Core.Filter
{
    /// <summary>
    /// �������������� ������ � ������ ������
    /// </summary>
    /// TODO : ����� ������� �������� �� ��������
    public class AdditionalFilter
    {
        /// <summary>
        /// ������������� ���������
        /// </summary>
        public long? DrugId { get; set; }

        //// �� ���� ��������� ��
        //public string OwnerRegistrationCertificateId { get; set; }

        //�� ���� ���������������
        public long? OwnerTradeMarkId { get; set; }

        // ��� ����������
        public long? PackerId { get; set; }

        //�������� ������ ��������
        public string Text { get; set; }

        //��������
        public string TradeName { get; set; }

        public string DrugClearId { get; set; }

        //�������������
        public string Manufacturer { get; set; }
        
        public string GZ_code { get; set; }

        public string GetFilter()
        {
            var mainCondition = new StringBuilder();
            var subConditions = new List<string>();

            if (DrugId.HasValue)
                subConditions.Add(string.Format("c.DrugId = {0}", DrugId));
 
            if (OwnerTradeMarkId>0)
                subConditions.Add(string.Format("c.OwnerTradeMarkId in (select Id from Classifier.Manufacturer where [Id] = '{0}')", OwnerTradeMarkId));

            if (PackerId>0)
                subConditions.Add(string.Format("c.PackerId in (select t.Id from Classifier.Manufacturer t where t.[id] = '{0}')", PackerId));

            if (!string.IsNullOrEmpty(Text))
            {
                Text = Text.Replace("*", "");
                Text = Text.Replace("'", "");
                Text = Text.Replace(",", "%' or drug.Text like '%");
                Text = string.Format("(drug.Text like '%{0}%')", Text);

                subConditions.Add(Text);
            }

            if (!string.IsNullOrEmpty(TradeName))
                subConditions.Add(string.Format("d.TradeNameId in (select Id from Classifier.TradeName where Value like '{0}')", TradeName.Replace("'", "").Replace("*","%")));

            if (!string.IsNullOrEmpty(Manufacturer))
                subConditions.Add(string.Format("drug.Manufacturer like '{0}'", Manufacturer.Replace("'", "").Replace("*", "%")));

            if (subConditions.Count > 0)
            {
                foreach (var where in subConditions)
                {
                    if (mainCondition.Length > 0)
                        mainCondition.Append(" and ");

                    mainCondition.Append(where);
                }

                mainCondition.Insert(0, "(");
                mainCondition.Append(")");
            }

            return mainCondition.ToString();
        }
    }
}