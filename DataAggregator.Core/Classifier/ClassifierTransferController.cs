using System;
using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{

    /// <summary>
    /// Данный класс актуализиует таблицу переходов, которую редактируют пользователи.
    /// Т.е. если запись 
    /// </summary>
    public class ClassifierTransferController
    {
        public static void Change(ProductionInfo from, ProductionInfo to, Guid userId, DrugClassifierContext context)
        {
            if (from == null || from.Id == to.Id)
                return;


            var transferFrom = context.ClassifierTransfer.Where(t => t.ClassifierIdFrom == from.Id).ToList();
            transferFrom.ForEach(t => context.ClassifierTransfer.Remove(t));

            var transferTo = context.ClassifierTransfer.Where(t => t.ClassifierIdTo == from.Id).ToList();
            transferTo.ForEach(t =>
            {
                t.ClassifierIdTo = to.Id;
                t.UserId = userId;
            });


        }


       
    }
}
