using DataAggregator.Domain.Model.EtalonPrice;
using DataAggregator.Domain.Model.OFD;
using System;

namespace DataAggregator.Web.Models.OFD
{
    public class PriceCurrentModel
    {
        public int? ClassifierId { get; set; }
        public int? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string INNGroup { get; set; }
        public int? INNGroupId { get; set; }
        public int? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public int? PackerId { get; set; }
        public string Packer { get; set; }
        public int? OwnerRegistrationCertificateId { get; set; }
        public string OwnerRegistrationCertificate { get; set; }
        public int? RetailDataSellingSum { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? PriceNew { get; set; }
        public decimal? PriceCalc { get; set; }
        public long? OFDSum { get; set; }
        public string StatusText { get; set; }
        public int? RetailDataAvgPrice { get; set; }
        public int? RetailDataMedianaPrice { get; set; }
        public int? OFDMediana50Price { get; set; }
        public int? OFDMediana65Price { get; set; }
        public int? RetailDataPrice { get; set; }
        public int? OFDPrice { get; set; }
        public string Comment { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid? UserIdUpdate { get; set; }
        public int? PriceRawMedianaRaw { get; set; }
        public int? PriceOFDMedianaRaw { get; set; }
        public int? PriceRaw65PercentileOFD { get; set; }
        public int? PriceOFD65PercentileOFD { get; set; }
        public string User { get; set; }
        public bool WithoutPrice { get; set; }
        public bool IsFractionalPackaging { get; set; }

        public string Type { get; set; }

        public decimal? PriceVED { get; set; }

        public bool ForChecking { get; set; }

        public string RegistrationCertificateNumber { get; set; }

        public bool Used { get; set; }

        public int? OFD1Mediana50Price { get; set; }
        public int? OFD1Mediana65Price { get; set; }
        public int? OFD2Mediana50Price { get; set; }
        public int? OFD2Mediana65Price { get; set; }
        public int? OFD3Mediana50Price { get; set; }
        public int? OFD3Mediana65Price { get; set; }
        public int? OFD4Mediana50Price { get; set; }
        public int? OFD4Mediana65Price { get; set; }
        public int? OFD5Mediana50Price { get; set; }
        public int? OFD5Mediana65Price { get; set; }
        public int? OFD6Mediana50Price { get; set; }
        public int? OFD6Mediana65Price { get; set; }
        public int? Retail1Mediana50Price { get; set; }
        public int? Retail1Mediana65Price { get; set; }
        public int? Retail2Mediana50Price { get; set; }
        public int? Retail2Mediana65Price { get; set; }
        public int? Retail3Mediana50Price { get; set; }
        public int? Retail3Mediana65Price { get; set; }
        public int? Retail4Mediana50Price { get; set; }
        public int? Retail4Mediana65Price { get; set; }
        public int? Retail5Mediana50Price { get; set; }
        public int? Retail5Mediana65Price { get; set; }
        public int? Web1Mediana50Price { get; set; }
        public int? Web1Mediana65Price { get; set; }
        public int? Web2Mediana50Price { get; set; }
        public int? Web2Mediana65Price { get; set; }
        public int? Web3Mediana50Price { get; set; }
        public int? Web3Mediana65Price { get; set; }
        public int? Web4Mediana50Price { get; set; }
        public int? Web4Mediana65Price { get; set; }

        public static PriceCurrentModel Create(PriceCurrentView model)
        {
            return ModelMapper.Mapper.Map<PriceCurrentModel>(model);
        }

    
    }
}