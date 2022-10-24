using System;
using System.Linq;
using System.Transactions;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Log;

namespace DataAggregator.Core.GoodsClassifier
{
    public class GoodsLogAction
    {
        public enum GoodsActionType
        {
            Add = 1,
            Change = 2,
            Merge = 3
        }

        public static void Log(DrugClassifierContext context, long goodsProductionInfoId, GoodsActionType type,Guid user)
        {
            GoodsActionLog log = new GoodsActionLog
            {
                ActionId = (long) type,
                GoodsProductionInfoId = goodsProductionInfoId,
                UserId = user

            };

            if (type != GoodsActionType.Add)
                log.Description = GetDescription(goodsProductionInfoId);

            context.GoodsActionLog.Add(log);
        }

        //Текстовое описание изменений
        private static string GetDescription(long id)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted}))
            {
                using (DrugClassifierContext context = new DrugClassifierContext("GoodsLogAction"))
                {
                    var description = context.GoodsProductionInfoDescription.Single(p => p.Id == id);
                    return description.Description;
                }
            }
        }

    }
}
