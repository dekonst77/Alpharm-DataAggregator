using System;
using AutoMapper;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;

namespace DataAggregator.Web.Mapper.Classifier
{
    public class ClassificationGenericModelProfile : Profile
    {
        public ClassificationGenericModelProfile()
        {
            CreateMap<ClassificationGeneric, ClassificationGenericModel>()
                .ForMember(dst => dst.TradeName,
                    opt => opt.MapFrom(s => s.TradeName.Value))
                .ForMember(dst => dst.InnGroup, opt => opt.MapFrom(src => src.INNGroup.Description))
                .ForMember(dst => dst.OwnerTradeMark,
                    opt => opt.MapFrom(s => s.OwnerTradeMark.Value))
                .ForMember(dst => dst.User,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dst => dst.Generic, opt => opt.MapFrom(src => src.Generic != null ? src.Generic.Value: String.Empty));

        }
    }
}