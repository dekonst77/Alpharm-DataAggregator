using System.Xml.Serialization;

namespace DataAggregator.Core.Filter
{
    /// <summary>
    /// Фильтр классификатора
    /// </summary>
    /// <remarks>true, если XmlSerializer создает атрибут xsi:nil; в противном случае — false.</remarks>
    [XmlRoot(IsNullable = false, Namespace = "")]
    public class ClassifierFilter
    {
        public ClassifierFilter()
        {
            Used = 1;
        }

        public long? ClassifierId { get; set; }

        public string TradeName { get; set; }

        public string RuNumber { get; set; }

        public long? TradeNameId { get; set; }

        public string OwnerTradeMark { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public string Packer { get; set; }

        public long? PackerId { get; set; }

        public string INNGroup { get; set; }

        public long? INNGroupId { get; set; }

        public string FormProduct { get; set; }

        public long? FormProductId { get; set; }

        public string DosageGroup { get; set; }

        public long? DosageGroupId { get; set; }

        public int? ConsumerPackingCount { get; set; }

        public long? DrugId { get; set; }

        [XmlElement(ElementName = "RegistrationCertificateNumber")]
        public string RegistrationNumber { get; set; }

        public long? Used { get; set; }

        public string Flags { get; set; }
        public string Comment { get; set; }
        public string RegNumber { get; set; }
    }
}
