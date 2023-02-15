using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataReport
{
    public class cField
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string sType { get; set; }
        public string SPR { get; set; }
        public bool IsEdit { get; set; }
        public bool IsKey { get; set; }
        public object Value { get; set; }
    }

    public class cCommand
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string command { get; set; }
        public string typec { get; set; }
    }
    public class cSPRItem
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
    public class cSPRItemStr
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class cSPR
    {
        public string Name { get; set; }
        public List<cSPRItem> Data { get; set; }
    }
}
