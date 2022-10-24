using AutoMapper;
using DataAggregator.Web.Models.Memberships;

namespace DataAggregator.Web
{
    public sealed class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentModel>(MemberList.None)
                .ForMember(dst => dst.ManagerName, opt => opt.MapFrom(src => src.Manager.FullName));
        }
    }
}