using System;
using System.Linq;
using System.Transactions;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Changes;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using IsolationLevel = System.Data.IsolationLevel;

namespace DataAggregator.Core.Classifier
{

    /// <summary>
    /// Данный класс отвечает за таблицу ClassifierReplacement
    /// </summary>
    public static class ClassifierReplacementController
    {
        public static void Add(ProductionInfo from, ProductionInfo to, Guid userId, DrugClassifierContext context)
        {


            //В случае если объединяют PI, который никогда не существовал в редакторе классификатора, то и записывать нечего
            if (from == null)
                return;

            //Но только в том случае, если ProductionInfo был пересоздан
            if (from.DrugId == to.DrugId)
                return;

            //1 - Добавляем запись

            //Из истории тянем старый идентификатор
            long fromClassifierInfoId = context.ClassifierInfoHistory.Single(i => i.ProductionInfoId == from.Id).ClassifierInfoId;
           
            //Из нового тяням новый id
            var toClassifierInfoId = context.ClassifierInfo.Single(i => i.ProductionInfoId == to.Id).Id;

            var classifier = new ClassifierReplacement
            {
                ClassifierIdFrom = fromClassifierInfoId,
                ClassifierIdTo = toClassifierInfoId,
                UserId = userId,
                DescriptionFrom = GetDescription(from),
                DescriptionTo = GetDescription(to)
            };

            context.ClassifierReplacement.Add(classifier);

        }

        //Текстовое описание изменений
        private static string GetDescription(ProductionInfo p)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {

                using (DrugClassifierContext context = new DrugClassifierContext())
                {

                    var description = context.ProductionInfoDescription.Single(a => a.Id == p.Id);

                    return description.Description;

                }
            }
        }
    }
}
