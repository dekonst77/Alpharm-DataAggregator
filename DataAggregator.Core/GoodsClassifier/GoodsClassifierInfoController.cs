using System;
using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.GoodsClassifier
{
    public class GoodsClassifierInfoController
    {
        public static GoodsClassifierInfoChange Change(GoodsProductionInfo from, GoodsProductionInfo to, Guid userId, DrugClassifierContext context)
        {
            GoodsClassifierInfoChange change = new GoodsClassifierInfoChange();

            ClassifierInfo goodsClassifierInfoFrom = null;

            if (from != null)
            {
                goodsClassifierInfoFrom = context.ClassifierInfo.SingleOrDefault(c => c.GoodsProductionInfoId == from.Id);
                if (goodsClassifierInfoFrom != null)
                    change.ClassifierInfoFromId = goodsClassifierInfoFrom.Id;
            }

            //Удаляем, если объединение или сохранение с новым Id
            if (from != null && from.GoodsId != to.GoodsId && goodsClassifierInfoFrom != null)
            {

                //Записываем в историю
                var classifierInfoHistory = new ClassifierInfoHistory
                {
                    ClassifierInfoId = (int)goodsClassifierInfoFrom.Id,
                    GoodsId = (int)from.GoodsId,
                    OwnerTradeMarkId = (int)from.OwnerTradeMarkId,
                    PackerId = (int)from.PackerId
                };


                context.ClassifierInfoHistory.Add(classifierInfoHistory);

                context.ClassifierInfo.Remove(goodsClassifierInfoFrom);
            }

            var classifierInfoTo = context.ClassifierInfo.SingleOrDefault(c => c.GoodsProductionInfoId == to.Id);

            if (classifierInfoTo == null)
            {
                classifierInfoTo = new ClassifierInfo { GoodsProductionInfoId = to.Id };

                var classifierInfoHistory = context.ClassifierInfoHistory.FirstOrDefault(p => p.GoodsId == to.GoodsId && p.OwnerTradeMarkId == to.OwnerTradeMarkId && p.PackerId == to.PackerId);
                
                if (classifierInfoHistory != null)
                    classifierInfoTo.Id = classifierInfoHistory.ClassifierInfoId;
                else
                {
                    context.ClassifierInfo.Add(classifierInfoTo);
                }
                context.SaveChanges();
            }

            context.SaveChanges();

            change.ClassifierInfoToId = classifierInfoTo.Id;


            return change;
        }

        public struct GoodsClassifierInfoChange
        {
            public long ClassifierInfoFromId { get; set; }
            public long ClassifierInfoToId { get; set; }
        }
    }
}
