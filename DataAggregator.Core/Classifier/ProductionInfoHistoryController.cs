using System;
using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{
    public class ProductionInfoHistoryController
    {
        public static void Change(ProductionInfo piold, ProductionInfo pinew, DrugClassifierContext context, Guid userId)
        {
            //Если замена коснулась OwnerTradeMark или Packer мы должны добавить в истории, что ранее для Drug была такая связка Pi1
            if (piold != null && piold.Id == pinew.Id &&
                (piold.OwnerTradeMarkId != pinew.OwnerTradeMarkId || piold.PackerId != pinew.PackerId))
            {


                var existsHistory = context.ProductionInfoHistory.Where(h => h.ProductionInfoId == piold.Id && h.DrugId != pinew.DrugId
                                                                       && h.OwnerTradeMarkId != pinew.OwnerTradeMarkId
                                                                       && h.PackerId != pinew.PackerId).ToList();


                //добавляется запись Pi с родительским элементов Pi2.

                ProductionInfoHistory newhistory = new ProductionInfoHistory();
                newhistory.ProductionInfo = pinew;
                newhistory.DrugId = piold.DrugId;
                newhistory.OwnerTradeMarkId = piold.OwnerTradeMarkId;
                newhistory.PackerId = piold.PackerId;

                context.ProductionInfoHistory.Add(newhistory);

                context.ClassifierInfo.Add(new ClassifierInfo() { ProductionInfoHistory = newhistory });


                //В случае, если запись Pi1 присутствовала в таблице истории,
                //то добавляется запись Pi1 с родительским элементом Pi2, 
                //при этом все дочерние элементы, ссылающиеся на Pi1 переносятся на Pi2.
                if (existsHistory.Any())
                {
                    existsHistory.ForEach(h => h.ProductionInfo = pinew);
                }


                //Удалеяем те записи, которые совпадают с новой, если поменяли сначала на одно, а потом вернули обратно
                //1,2,3 -> 1,4,3 -> 1,2,3 
                var forDelete = context.ProductionInfoHistory.Where(h => h.ProductionInfoId == piold.Id
                                                                       && h.DrugId == pinew.DrugId
                                                                       && h.OwnerTradeMarkId == pinew.OwnerTradeMarkId
                                                                       && h.PackerId == pinew.PackerId).ToList();

                //Удаляем из ProductionInfoHistory
                if (forDelete.Any())
                {
                    forDelete.ForEach(p => context.ProductionInfoHistory.Remove(p));
                }

                //Удаляем из ClassifierInfo
                foreach (var productionInfoHistory in forDelete)
                {

                    ClassifierInfoController.ChangeHistory(productionInfoHistory, null, userId, context);
                    ClassifierTransferController.ChangeHistory(productionInfoHistory, null, userId, context);
                }



            }
            //значит запись удаляется и мы должны удалить всю историю и удалить из ClassifierInfo
            else if (piold != null && piold.Id != pinew.Id)
            {
                var forDelete = context.ProductionInfoHistory.Where(h => h.ProductionInfoId == piold.Id).ToList();

                //Удаляем из ClassifierInfo
                foreach (var productionInfoHistory in forDelete)
                {

                    ClassifierInfoController.ChangeHistory(productionInfoHistory, null, userId, context);
                    ClassifierTransferController.ChangeHistory(productionInfoHistory, null, userId, context);
                }
            }
        }
    }
}