using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer
{
    /// <summary>
    /// Информация о результате поиска Drug
    /// </summary>
    public class DrugSearchInfo
    {
        public string TableName { get; set; }

        public long IdSyn { get; set; }

        public long IdOriginal { get; set; }

        public int Rank { get; set; }
    }
}
