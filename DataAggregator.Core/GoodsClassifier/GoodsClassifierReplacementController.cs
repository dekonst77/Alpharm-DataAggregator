using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Changes;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;

namespace DataAggregator.Core.GoodsClassifier
{
    /// <summary>
    /// Данный класс отвечает за таблицу ClassifierReplacement
    /// </summary>
    public static class GoodsClassifierReplacementController
    {
        internal static void Add(long classifierInfoFrom, long classifierInfoTo, Guid userId, DrugClassifierContext context)
        {
            //В случае если объединяют PI, который никогда не существовал в редакторе классификатора, то и записывать нечего
            if (classifierInfoFrom == 0)
                return;

            if (classifierInfoTo == 0)
                return;

            //Но только в том случае, если ProductionInfo был пересоздан
            if (classifierInfoFrom == classifierInfoTo)
                return;

            var classifier = new ClassifierReplacement
            {
                ClassifierIdFrom = classifierInfoFrom,
                ClassifierIdTo = classifierInfoTo,
                DescriptionFrom = String.Empty,
                DescriptionTo = String.Empty,
                UserId = userId
            };

            context.ClassifierReplacement.Add(classifier);

            //сразу вставляем маску на основании replace
            var mask = new Mask
            {
                FromClassifierId = classifierInfoFrom,
                ToClassifierId = classifierInfoTo,
                DateInsert = DateTime.Now,
                Manual = false,
                Use = true,
                UserReplaceId = userId
            };

            context.Mask.Add(mask);

            context.SaveChanges();
        }

       
    }
}
