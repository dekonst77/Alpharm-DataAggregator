﻿using System;
using System.Linq;
using DataAggregator.Core.GoodsClassifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Changes;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;

namespace DataAggregator.Core.Classifier
{
    public class ClassifierInfoController
    {
        public static void Change(ProductionInfo from, ProductionInfo to, Guid userId, DrugClassifierContext context)
        {

            ClassifierInfo classifierInfoFrom = null;

            //Если это добавление 
            if (from != null)
            {
                classifierInfoFrom = context.ClassifierInfo.SingleOrDefault(c => c.ProductionInfoId == from.Id);

                if (classifierInfoFrom == null)
                    throw new ApplicationException("Ошибка: исходного ClassifierId не существует");

                if (from.DrugId != to.DrugId ||
                    from.OwnerTradeMarkId != to.OwnerTradeMarkId ||
                    from.PackerId != to.PackerId)
                {
                    //Если такой записи еще не было, то записываем
                    if (!context.ClassifierInfoHistory.Any(h => h.ClassifierInfoId == (int)classifierInfoFrom.Id &&
                                                              h.DrugId == (int)from.DrugId &&
                                                              h.OwnerTradeMarkId == (int)from.OwnerTradeMarkId &&
                                                              h.PackerId == (int)from.PackerId))
                    {

                        //Записываем в историю
                        var classifierInfoHistory = new ClassifierInfoHistory
                        {
                            ClassifierInfoId = (int)classifierInfoFrom.Id,
                            DrugId = (int)from.DrugId,
                            OwnerTradeMarkId = (int)from.OwnerTradeMarkId,
                            PackerId = (int)from.PackerId
                        };

                        context.ClassifierInfoHistory.Add(classifierInfoHistory);
                    }
                }

                //Удаляем, если объединение или сохранение с новым LKCUContext
                if (from.Id != to.Id)
                {
                    var cidel = context.ClassifierPacking.Where(w => w.ClassifierId == classifierInfoFrom.Id);
                    context.ClassifierPacking.RemoveRange(cidel);
                    context.ClassifierInfo.Remove(classifierInfoFrom);
                }
            }

            ClassifierInfo classifierInfoTo = context.ClassifierInfo.SingleOrDefault(c => c.ProductionInfoId == to.Id);

            if (classifierInfoTo == null)
            {
                classifierInfoTo = new ClassifierInfo { ProductionInfoId = to.Id };

                //var classifierInfoHistory = context.ClassifierInfoHistory.FirstOrDefault(p =>
                //   p.DrugId == to.DrugId && p.OwnerTradeMarkId == to.OwnerTradeMarkId &&
                //   p.PackerId == to.PackerId);

                //if (classifierInfoHistory != null)
                //    classifierInfoTo.Id = classifierInfoHistory.ClassifierInfoId;
                //else
                //{
                    context.ClassifierInfo.Add(classifierInfoTo);
                //}

                context.SaveChanges();
            }

            //Записываем в таблицу ClassifierReplacement
            //В случае если объединяют PI, который никогда не существовал в редакторе классификатора, то и записывать нечего
            if (classifierInfoFrom != null && classifierInfoFrom.Id != classifierInfoTo.Id)
            {
               
                var classifier = new ClassifierReplacement
                {
                    ClassifierIdFrom = (int)classifierInfoFrom.Id,
                    ClassifierIdTo = (int)classifierInfoTo.Id,
                    UserId = userId
                };

                context.ClassifierReplacement.Add(classifier);

                context.SaveChanges();

                //сразу вставляем маску на основании replace
                GoodsClassifierReplacementController.Add(classifierInfoFrom.Id, classifierInfoTo.Id, userId, context);
            }

            context.SaveChanges();
        }
    }
}
