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
                ActionId = (long)type,
                ProductionInfoId = productionInfoId,
                UserId = user

            };

            if (type != ActionType.Add)
                log.Description = GetDescription(productionInfoId);

            context.ActionLog.Add(log);

        }

        /// <summary>
        /// Текстовое описание изменений
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetDescription(long id)
        {
            using (DrugClassifierContext context = new DrugClassifierContext("LogAction"))
            {
                return context.GetProductionInfoDescription_Result(id);
            }
        }


    }
}
