using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DataAggregator.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDataAggregatorService
    {
        [OperationContract]
        void GetDrugs(string drugFilter, Guid userGuid);

        [OperationContract]
        void GetDrugsForAnalyze(long robotId, int currentVersion, int count);

        [OperationContract]
        void GetPurchases(int count, Guid userGuid);

        [OperationContract]
        void GetPurchasesSupplierResult(int count, Guid userGuid);
        
        [OperationContract]
        void GetPurchasesWithContracts(int count, Guid userGuid, bool IsKK);

        [OperationContract]
        void GetPurchasesByFilter(string sql, Guid userGuid);

        [OperationContract]
        void SetDrugs(Guid userId);
    }
}
