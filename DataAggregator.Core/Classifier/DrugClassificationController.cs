using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{
    //Данный контроллер отвечает за таблицу DrugClassification в которой хранятся характеристики связок Drug + OwnerTradeMark
    public class DrugClassificationController
    {
        public static void Change(ProductionInfo piold, ProductionInfo pinew, DrugClassifierContext context)
        {

            //Данные действия следует проводить, только в случае, если изменился Drug или OwnerTradeMark

            if(piold != null && pinew.DrugId == piold.DrugId && pinew.OwnerTradeMarkId == piold.OwnerTradeMarkId)
                return;


            //если Drug + OnwerTradeMark в pinew - новый, то все характеристики переносяться со старого, если он есть
            
            var drugClassificationOld = piold != null
                ? context.DrugClassification.SingleOrDefault(c => c.DrugId == piold.DrugId &&
                                                                  c.OwnerTradeMarkId == piold.OwnerTradeMarkId)
                : null;

            var drugClassificationNew =
                context.DrugClassification.SingleOrDefault(c => c.DrugId == pinew.DrugId &&
                                                            c.OwnerTradeMarkId == pinew.OwnerTradeMarkId);

            //Если для новой связки еще не было характеристик, то их нужно добавить
            if (drugClassificationNew == null)
            {
                //Если характеристики были у старой, то переносим все характеристики на новую запись
                drugClassificationNew = drugClassificationOld != null
                    ? drugClassificationOld.Copy()
                    : new DrugClassification();

                drugClassificationNew.DrugId = pinew.DrugId;
                drugClassificationNew.OwnerTradeMarkId = pinew.OwnerTradeMarkId;



                context.DrugClassification.Add(drugClassificationNew);

            }

            if (piold != null && drugClassificationOld != null)
            {
                //Если у старого больше нет других таких же Drug + OnwerTradeMark, то характеристики с такой связки удаляются
                var count = context.ProductionInfo.Count(p => p.DrugId == piold.DrugId &&
                                                              p.OwnerTradeMarkId == piold.OwnerTradeMarkId &&
                                                              p.Id != piold.Id);

                if (count == 0)
                {
                    context.DrugClassification.Remove(drugClassificationOld);
                }
            }

        }




    }
}