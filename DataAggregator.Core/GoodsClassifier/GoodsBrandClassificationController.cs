using System.Linq;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.GoodsClassifier
{
    public static class GoodsBrandClassificationController
    {
        public static void Change(GoodsProductionInfo piold, GoodsProductionInfo pinew, DrugClassifierContext context, bool isRecreate)
        {
            if (piold == null || isRecreate)
                return;

            //Данные действия следует проводить, только в случае, если изменился Drug или OwnerTradeMark
            long goodsTradeNameOldId = context.Goods.Single(d => d.Id == piold.GoodsId).GoodsTradeNameId;
            long goodsTradeNameNewId = context.Goods.Single(d => d.Id == pinew.GoodsId).GoodsTradeNameId;


            //Если ничего не поменялось
            if (goodsTradeNameNewId == goodsTradeNameOldId && pinew.OwnerTradeMarkId == piold.OwnerTradeMarkId)
                return;

            //если TradeNameId + OnwerTradeMark в pinew - новый, то все характеристики переносяться со старого, если он есть
            var brandClassificationOld = context.GoodsBrandClassification.SingleOrDefault(c => c.GoodsTradeNameId == goodsTradeNameOldId &&
                                                                                          c.OwnerTradeMarkId == piold.OwnerTradeMarkId);

            if (brandClassificationOld != null)
            {
                //Если у старого больше нет других таких же TradeName + OnwerTradeMark, то характеристики с такой связки удаляются
                var count = context.GoodsProductionInfo.Count(p => p.Goods.GoodsTradeNameId == goodsTradeNameOldId &&
                                                              p.OwnerTradeMarkId == piold.OwnerTradeMarkId &&
                                                              p.Id != piold.Id);

                if (count == 0)
                {
                    context.GoodsBrandClassification.Remove(brandClassificationOld);
                }
            }
        }
    }
}
