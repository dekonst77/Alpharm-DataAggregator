using DataAggregator.Domain.Model.Retail.CTM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.Retail.CTM
{
    public class CTMModel
    {
        public long Id { get; set; }
        public long ClassifierID { get; set; }
        public long TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? NetworkID { get; set; }
        public string NetworkName { get; set; }
        public bool DeBrikingState { get; set; }
        public string DeBrikingStateName { get; set; }
        public DateTime? period { get; set; }
        public string comment { get; set; }
        public static CTMModel Create(CTMView model)
        {
            return ModelMapper.Mapper.Map<CTMModel>(model);
        }
    }
}