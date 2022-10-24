using System;
using System.Data.SqlClient;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.GoodsClassifier
{
    public static class GoodsProductionInfoController
    {

        //Действия при изменении ProductionInfo
        public static GoodsClassifierInfoController.GoodsClassifierInfoChange ChangeProductionInfo(GoodsProductionInfo from, GoodsProductionInfo to, Guid userId, DrugClassifierContext context, bool isRecreate = false)
        {
            //Изменяем таблицу ClassifierInfo
            var change = GoodsClassifierInfoController.Change(from, to, userId, context);

            //Записываем в таблицу ClassifierReplacement
            GoodsClassifierReplacementController.Add(change.ClassifierInfoFromId, change.ClassifierInfoToId, userId, context);

            //Учитываем изменения в BrandClassification
            GoodsBrandClassificationController.Change(from, to, context, isRecreate);

            //Перепривязываем данные при изменении привязки
            GoodsReClassifierController.ReClassifier(from, to, userId, context);

            return change;

        }


        public static void PublishFull(long changeClassifierInfoToId, DrugClassifierContext context)
        {
            context.Database.CommandTimeout = 6000;

            context.Database.ExecuteSqlCommand(@"[dbo].[PublishFull] @ClassifierId",
                new SqlParameter("@ClassifierId", changeClassifierInfoToId));

        }
    }
}
