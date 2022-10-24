using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DataAggregator;

namespace DataAggregator.Domain.Utils
{
    public class Logger : ErrorLog
    {
        public void Save()
        {
            using (var context = new DataAggregatorContext("Logger"))
            {
                context.ErrorLog.Add(this);

                context.SaveChanges();
            }
        }
    }
}
