using AutoMapper;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Web.Models.Retail.GoodsCountRuleEditor;

namespace DataAggregator.Web
{
    public class GoodsCountRuleProfile : Profile
    {
        public GoodsCountRuleProfile()
        {
            CreateMap<GoodsCountRuleView, GoodsCountRuleModel>()
                .ForMember(dst => dst.Region,
                    opt => opt.MapFrom(src => string.Format("{0} {1}", src.RegionCode, src.RegionFullName)));
        }
    }
}