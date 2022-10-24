using AutoMapper;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Web.Models.Retail.CountRuleEditor;

namespace DataAggregator.Web
{
    public class CountRuleProfile : Profile
    {
        public CountRuleProfile()
        {
            CreateMap<CountRuleView, CountRuleModel>()
                .ForMember(dst => dst.Region,
                    opt => opt.MapFrom(src => string.Format("{0} {1}", src.RegionCode, src.RegionFullName)))
                .ForMember(dst => dst.InUsed,
                    opt => opt.Ignore())
                .ForMember(dst => dst.OutUsed,
                    opt => opt.Ignore())
                .ForMember(dst => dst.Flag,
                opt => opt.Ignore());
        }
    }
}