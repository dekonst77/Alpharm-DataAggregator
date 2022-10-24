using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;

namespace DataAggregator.Core.GoodsClassifier
{
    /// <summary>
    /// Данный контроллер отвечает за перепривязку данных
    /// </summary>
    public class GoodsReClassifierController
    {
        /// <summary>
        /// Перепривязываем данные и заносим информацию в журнал замен внутри хранимой процедуры
        /// </summary>
        /// <param name="fromProductionInfo"></param>
        /// <param name="toProductionInfo"></param>
        /// <param name="userId"></param>
        public static void ReClassifier(GoodsProductionInfo fromProductionInfo, GoodsProductionInfo toProductionInfo, Guid userId, DrugClassifierContext context)
        {
            //Значит такой ProductionInfo не было и мы делаем merge на другой
            if (fromProductionInfo == null)
                return;

            if (fromProductionInfo.GoodsId == toProductionInfo.GoodsId &&
                fromProductionInfo.OwnerTradeMarkId == toProductionInfo.OwnerTradeMarkId &&
                fromProductionInfo.PackerId == toProductionInfo.PackerId)
                return;

            context.Database.CommandTimeout = 6000;

            context.Database.ExecuteSqlCommand(@"[GoodsSystematization].[GoodsReplacementProcedure] @FromGoodsId, @FromOwnerTradeMarkId,  @FromPackerId, @FromProductionInfoId,
                                                                                                    @ToGoodsId, @ToOwnerTradeMarkId, @ToPackerId, @ToProductionInfoId, 
                                                                                                    @UserId",
                new SqlParameter("@FromGoodsId", fromProductionInfo.GoodsId),
                new SqlParameter("@FromOwnerTradeMarkId", fromProductionInfo.OwnerTradeMarkId),
                new SqlParameter("@FromPackerId", fromProductionInfo.PackerId),
                new SqlParameter("@FromProductionInfoId", fromProductionInfo.Id),
                new SqlParameter("@ToGoodsId", toProductionInfo.GoodsId),
                new SqlParameter("@ToOwnerTradeMarkId", toProductionInfo.OwnerTradeMarkId),
                new SqlParameter("@ToPackerId", toProductionInfo.PackerId),
                new SqlParameter("@ToProductionInfoId", toProductionInfo.Id),
                new SqlParameter("@UserId", userId));



        }
    }
}
