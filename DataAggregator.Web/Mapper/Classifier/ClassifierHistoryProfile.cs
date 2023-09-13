using AutoMapper;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;

namespace DataAggregator.Web.Mapper.OFD
{
    public class ClassifierHistoryProfile : Profile
    {
        public ClassifierHistoryProfile()
        {
            CreateMap<ClassifierHistoryView, ClassifierHistoryModel>();     
          
        }

    }
}