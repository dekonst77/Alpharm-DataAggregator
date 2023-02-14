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

        public static void Log(DrugClassifierContext context, long goodsProductionInfoId, GoodsActionType type, Guid user)
        {
            GoodsActionLog log = new GoodsActionLog
            {
                ActionId = (long)type,
                GoodsProductionInfoId = goodsProductionInfoId,
                UserId = user

            };

            if (type != GoodsActionType.Add)
                log.Description = GetDescription(goodsProductionInfoId);

            context.GoodsActionLog.Add(log);
        }

        /// <summary>
        /// Текстовое описание изменений
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetDescription(long id)
        {
            using (DrugClassifierContext context = new DrugClassifierContext("GoodsLogAction"))
            {
                return context.GetGoodsProductionInfoDescription_Result(id);
            }
        }

    }
}
