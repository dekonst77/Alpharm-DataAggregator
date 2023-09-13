using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System;

namespace DataAggregator.Web.Models.Classifier
{
    public class ClassifierHistoryModel
    {

        public int Id { get; set; }
        public int ClassifierId { get; set; }
        public int DrugId { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public int PackerId { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public DateTime When { get; set; }
        public string Flag { get; set; }

        public static ClassifierHistoryModel Create(ClassifierHistoryView model)
        {
            return ModelMapper.Mapper.Map<ClassifierHistoryModel>(model);
        }

    }
}