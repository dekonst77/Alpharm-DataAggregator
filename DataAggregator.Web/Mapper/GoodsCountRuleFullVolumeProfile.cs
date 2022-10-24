using AutoMapper;
using DataAggregator.Domain.Model.GoodsData;
using DataAggregator.Web.Models.Retail.GoodsCountRuleFullVolumeEditor;

namespace DataAggregator.Web
{
    public sealed class GoodsCountRuleFullVolumeProfile : Profile
    {
        public GoodsCountRuleFullVolumeProfile()
        {
            CreateMap<GoodsCountRuleFullVolumeView, GoodsCountRuleFullVolumeModel>();

            CreateMap<GoodsCountRuleFullVolumeModel, GoodsCountRuleFullVolume>()
                .ForMember(dst => dst.ChangeUserId, opt => opt.Ignore());
        }
    }
}