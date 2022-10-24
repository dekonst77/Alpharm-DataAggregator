using AutoMapper;
using DataAggregator.Domain.Model.EtalonPrice;
using DataAggregator.Web.Models.OFD;

namespace DataAggregator.Web.Mapper.OFD
{
    public class PriceCurrentProfile : Profile
    {
        public PriceCurrentProfile()
        {
            CreateMap<PriceCurrentView, PriceCurrentModel>();
            CreateMap<PriceCurrentView_v2, PriceCurrentModel_v2>();

        }

    }
}