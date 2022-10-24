using AutoMapper;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Domain.Model.RetailCalculation;
using DataAggregator.Web.Models.Retail.CountRuleEditor;
using DataAggregator.Web.Models.RetailCalculation;

namespace DataAggregator.Web.Mapper.RetailCalculation
{
    public class LauncherProfile : Profile
    {
        public LauncherProfile()
        {
            CreateMap<Launcher, LauncherModel>()
                .ForMember(dst => dst.Status,
                    opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dst => dst.Process,
                    opt => opt.MapFrom(src => src.Process.Name))
                .ForMember(dst => dst.User,
                    opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}