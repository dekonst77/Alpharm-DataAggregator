using AutoMapper;
using DataAggregator.Domain.Model.Retail.Report;
using DataAggregator.Web.Models.Retail.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Mapper.Retail
{
    public sealed class ReportLauncherProfile : Profile
    {
        public ReportLauncherProfile()
        {
            CreateMap<ReportLauncher, ReportLauncherModel>()
               .ForMember(rm => rm.ReportName, opt => opt.MapFrom(src => src.Report.Name))
               .ForMember(rm => rm.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
               .ForMember(rm => rm.StatusName, opt => opt.MapFrom(src => src.Status.Name))
               .ForMember(rm => rm.DateEnd, opt => opt.MapFrom(src => src.DateEnd))
               .ForMember(rm => rm.ErrorMessage, opt => opt.MapFrom(src => src.ErrorMessage))
               .ForMember(rm => rm.UserId, opt => opt.Ignore())
               .ForMember(rm => rm.SendSelf, opt => opt.Ignore());

            CreateMap<ReportLauncherModel, ReportLauncher>()
                .ForMember(rm => rm.Report, opt => opt.Ignore())
                .ForMember(rm => rm.Status, opt => opt.Ignore())
                .ForMember(rm => rm.User, opt => opt.Ignore())
                .ForMember(rm => rm.DateEnd, opt => opt.Ignore())
                .ForMember(rm => rm.UserId, opt => opt.Ignore())
                .ForMember(rm => rm.ErrorMessage, opt => opt.Ignore());         

        }
    }
}