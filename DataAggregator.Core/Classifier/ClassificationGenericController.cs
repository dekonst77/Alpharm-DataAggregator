using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using System.Linq;

namespace DataAggregator.Core.Classifier
{
    public class ClassificationGenericController
    {
        public static void Change(ProductionInfo piold, ProductionInfo pinew, DrugClassifierContext context)
        {
            //если Drug + OnwerTradeMark в pinew - новый, то все характеристики переносяться со старого, если он есть

            var classificationGenericOld = piold != null
                ? context.ClassificationGeneric.SingleOrDefault(c => c.TradeNameId == piold.Drug.TradeNameId &&
                                                                     c.INNGroupId == piold.Drug.INNGroupId &&
                                                                     c.OwnerTradeMarkId == piold.OwnerTradeMarkId)
                : null;

            var classificationGenericNew = 
                context.ClassificationGeneric.SingleOrDefault(c => c.TradeNameId == pinew.Drug.TradeNameId &&
                                                                   c.INNGroupId == pinew.Drug.INNGroupId &&
                                                                   c.OwnerTradeMarkId == pinew.OwnerTradeMarkId);

            //Если для новой связки еще не было характеристик, то их нужно добавить
            if (classificationGenericNew == null && pinew.Drug.INNGroupId.HasValue)
            {
                //Если характеристики были у старой, то переносим все характеристики на новую запись
                classificationGenericNew = new ClassificationGeneric();

                classificationGenericNew.TradeNameId = pinew.Drug.TradeNameId;
                classificationGenericNew.INNGroupId = pinew.Drug.INNGroupId.Value;
                classificationGenericNew.OwnerTradeMarkId = pinew.OwnerTradeMarkId;
                context.ClassificationGeneric.Add(classificationGenericNew);
            }

            if (piold != null && classificationGenericOld != null)
            {
                //Если у старого больше нет других таких же TradeName + INNGroup + OnwerTradeMark, то характеристики с такой связки удаляются
                var count = context.ProductionInfo.Count(p => p.Drug.TradeNameId == piold.Drug.TradeNameId &&
                                                              p.Drug.INNGroupId == piold.Drug.INNGroupId &&
                                                              p.OwnerTradeMarkId == piold.OwnerTradeMarkId);

                if (count == 0)
                {
                    context.ClassificationGeneric.Remove(classificationGenericOld);
                }
            }
        }
    }
}
