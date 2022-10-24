using DataAggregator.Core.Filter;
using DataAggregator.Domain.Model.DrugClassifier.Stat;
using System.Collections.Generic;

namespace DataAggregator.Web.Models.Systematization
{
    public class DrugFilterJson
    {
        public int Count { get; set; }
     
        public DrugClearWorkStat DrugClearWorkStat { get; set; }

        public List<RobotStat> RobotStat { get; set; } 

        public List<UserWorkStat> UserWorkStat { get; set; }

        public List<DataTypeStat> DataTypeStat { get; set; }
        public List<DateStat> DateStat { get; set; }
        public List<CategoryStatDrugView> CategoryStat { get; set; }
        public List<PrioritetStat> PrioritetStat { get; set; }

        public AdditionalFilter Additional { get; set; }

        public DrugFilterJson()
        {
            RobotStat=new List<RobotStat>();
            UserWorkStat=new List<UserWorkStat>();
            DataTypeStat=new List<DataTypeStat>();
            DateStat = new List<DateStat>();
            CategoryStat = new List<CategoryStatDrugView>();
            PrioritetStat = new List<PrioritetStat>();
        }
    }
}