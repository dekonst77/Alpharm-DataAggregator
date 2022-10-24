using System;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Web.Models.Classifier
{
    public class ClassificationGenericModel
    {

        public int Id { get; set; }
        public string TradeName { get; set; }
        public string InnGroup { get; set; }
        public string OwnerTradeMark { get; set; }
        public string User { get; set; }
        public DateTime? DateEdit { get; set; }
        public string Generic { get; set; }



        public static ClassificationGenericModel Create(ClassificationGeneric model)
        {
            return ModelMapper.Mapper.Map<ClassificationGenericModel>(model);
        }
    }
}