using System;
using System.Linq;
using System.Transactions;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Log;


namespace DataAggregator.Core.Classifier
{
    public class LogAction
    {

        public enum ActionType
        {
            Add = 1,
            Change = 2,
            Merge = 3
        }

        public static void Log(DrugClassifierContext context, long productionInfoId, ActionType type, Guid user)
        {
            ActionLog log = new ActionLog
            {
                ActionId = (long) type,
                ProductionInfoId = productionInfoId,
                UserId = user
             
            };

            if (type != ActionType.Add)
                log.Description = GetDescription(productionInfoId);

            context.ActionLog.Add(log);
          
        }


        //Текстовое описание изменений
        private static string GetDescription(long id)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted}))
            {

                using (DrugClassifierContext context = new DrugClassifierContext("LogAction"))
                {

                    var description = context.ProductionInfoDescription.Single(p => p.Id == id);

                    return description.Description;

                    //return string.Format(@"{0} {1} OwnerTrademark: {2} {3} Packer: {4} {5}",
                    //    drug.TradeName,
                    //    drug.DrugDescription,
                    //    drug.OwnerTradeMarkId,
                    //    drug.OwnerTradeMark,
                    //    drug.PackerId,
                    //    drug.Packer);
                }
            }
        
    }

     
    }
}
