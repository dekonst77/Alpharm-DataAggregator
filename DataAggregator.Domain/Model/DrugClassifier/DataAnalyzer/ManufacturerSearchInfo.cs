using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer
{
    /// <summary>
    /// Информация о результате поиска Manufacturer
    /// </summary>
    public class ManufacturerSearchInfo
    {
        //public string TableName { get; set; }

        //public long IdSyn { get; set; }

        //public int Rank { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }
    }
}
