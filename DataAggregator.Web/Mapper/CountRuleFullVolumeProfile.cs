using AutoMapper;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Web.Models.Retail.CountRuleFullVolumeEditor;

namespace DataAggregator.Web
{
    public sealed class CountRuleFullVolumeProfile : Profile
    {
        public CountRuleFullVolumeProfile()
        {
            CreateMap<CountRuleFullVolumeView, CountRuleFullVolumeModel>();

            CreateMap<CountRuleFullVolumeModel, CountRuleFullVolume>()
                .ForMember(dst => dst.ChangeUserId, opt => opt.Ignore());
        }
    }
}