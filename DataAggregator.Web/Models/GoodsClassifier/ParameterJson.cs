using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Web.Models.GoodsClassifier
{
    public class ParameterJson
    {
        public long Id { get; set; }

        public long? ParentId { get; set; }

        public long? ParameterGroupId { get; set; }

        public string Value { get; set; }

        public ParameterJson(Parameter parameter)
        {
            this.Id = parameter.Id;
            this.ParentId = parameter.ParentId;
            this.ParameterGroupId = parameter.ParameterGroupId;
            this.Value = parameter.Value;
        }
    }
}