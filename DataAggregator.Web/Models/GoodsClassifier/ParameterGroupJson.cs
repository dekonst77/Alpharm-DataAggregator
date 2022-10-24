using System.Collections.Generic;

namespace DataAggregator.Web.Models.GoodsClassifier
{
    public class ParameterGroupJson
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<ParameterJson> ParametersList { get; set; }

        public string SelectedParameterValue { get; set; }
    }
}