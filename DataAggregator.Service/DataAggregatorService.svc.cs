using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataAggregator.Domain.DAL;

namespace DataAggregator.Service
{
    public class DataAggregatorService : IDataAggregatorService
    {
        private static readonly object LockDrugsData = new Object();
        private static readonly object LockGetPurchasesData = new Object();

        public void GetDrugs(string drugFilter, Guid userGuid)
        {
            if (string.IsNullOrEmpty(drugFilter))
            {
                throw new ArgumentNullException("drugFilter");
            }

            using (var context = new DrugClassifierContext("DataAggregatorService"))
            {
                lock (LockDrugsData)
                {
                    context.GetDrugs(drugFilter, userGuid);
                }
            }
        }

        public void GetDrugsForAnalyze(long robotId, int currentVersion, int count)
        {
            if (robotId <= 0)
            {
                throw new ArgumentNullException("robotId");
            }
            if (count <= 0)
            {
                throw new ArgumentNullException("count");
            }

            using (var context = new DrugClassifierContext("DataAggregatorService"))
            {
                lock (LockDrugsData)
                {
                    context.GetDrugsForAnalyze(robotId, currentVersion, count);
                }
            }
        }


        public void GetPurchasesSupplierResult(int count, Guid userGuid)
        {
            using (var context = new GovernmentPurchasesContext("DataAggregatorService"))
            {
                lock (LockGetPurchasesData)
                {
                    context.GetPurchasesSupplierResult(count, userGuid);
                }
            }
        }

        public void GetPurchases(int count, Guid userGuid)
        {
            using (var context = new GovernmentPurchasesContext("DataAggregatorService"))
            {
                lock (LockGetPurchasesData)
                {
                    context.GetPurchases(count, userGuid);
                }
            }
        }

        public void GetPurchasesWithContracts(int count, Guid userGuid, bool IsKK)
        {
            using (var context = new GovernmentPurchasesContext("DataAggregatorService"))
            {
                lock (LockGetPurchasesData)
                {
                    int i = 0;
                    i++;
                    context.GetPurchasesWithContracts(count, userGuid, IsKK);
                }
            }
        }

        public void GetPurchasesByFilter(string sql, Guid userGuid)
        {
            using (var context = new GovernmentPurchasesContext("DataAggregatorService"))
            {
                lock (LockGetPurchasesData)
                {
                    context.GetPurchasesByFilter(sql, userGuid);
                }
            }
        }

        public void SetDrugs(Guid userId)
        {
            using (var context = new DrugClassifierContext("DataAggregatorService"))
            {
                lock (LockDrugsData)
                {
                    context.SetDrugs(userId);
                }
            }
        }
    }
}
