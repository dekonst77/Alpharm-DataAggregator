using AutoMapper;
using DataAggregator.Domain.Model.LPU;
using DataAggregator.Web.Models.LPU;

namespace DataAggregator.Web.Mapper.LPU
{
    public class LPUModelProfile : Profile
    {
        public LPUModelProfile()
        {
            CreateMap<LPUView, LPUModel>();
            CreateMap<LPUPointView, LPUPointModel>();            
            CreateMap<LPULicensesView, LPULicensesModel>();
            CreateMap<DataAggregator.Domain.Model.LPU.Department, DepartmentModel>();
        }
              
    }
}