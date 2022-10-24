using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{
   /* public class BrandClassificationController
    {
        public static void Change(ProductionInfo piold, ProductionInfo pinew, DrugClassifierContext context)
        {

          

            if (piold == null)
                return;
            

            //Данные действия следует проводить, только в случае, если изменился Drug или OwnerTradeMark
            long tradeNameOldId = context.Drugs.Single(d => d.Id == piold.DrugId).TradeNameId;
            long tradeNameNewId = context.Drugs.Single(d => d.Id == pinew.DrugId).TradeNameId;


            //Если ничего не поменялось
            if (tradeNameNewId == tradeNameOldId && pinew.OwnerTradeMarkId == piold.OwnerTradeMarkId)
                return;


            //если TradeNameId + OnwerTradeMark в pinew - новый, то все характеристики переносяться со старого, если он есть
            var brandClassificationOld = context.BrandClassification.SingleOrDefault(c => c.TradeNameId == tradeNameOldId &&
                                                                  c.OwnerTradeMarkId == piold.OwnerTradeMarkId);
         

            if (brandClassificationOld != null)
            {
                //Если у старого больше нет других таких же TradeName + OnwerTradeMark, то характеристики с такой связки удаляются
                var count = context.ProductionInfo.Count(p => p.Drug.TradeNameId == tradeNameOldId &&
                                                              p.OwnerTradeMarkId == piold.OwnerTradeMarkId &&
                                                              p.Id != piold.Id);

                if (count == 0)
                {
                    context.BrandClassification.Remove(brandClassificationOld);
                }
            }

        }
    }*/
}
