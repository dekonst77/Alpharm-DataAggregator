using AutoMapper;
using DataAggregator.Domain.Model.OFD;
using DataAggregator.Web.Models.OFD;

namespace DataAggregator.Web.Mapper.OFD
{
    public class PriceEtalonProfile : Profile
    {
        public PriceEtalonProfile()
        {
            CreateMap<PriceEtalonView, PriceEtalonModel>();
            CreateMap<Classifier_ExternalView, PriceEtalonModel>()
             .ForMember(rm => rm.Year, opt => opt.Ignore())
             .ForMember(rm => rm.Month, opt => opt.Ignore())
             .ForMember(rm => rm.Price, opt => opt.Ignore())
             .ForMember(rm => rm.Id, opt => opt.Ignore());
        }

    }
}