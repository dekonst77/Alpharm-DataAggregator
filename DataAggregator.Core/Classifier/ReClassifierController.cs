using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Classifier
{
    /// <summary>
    /// Данный контроллер отвечает за перепривязку данных
    /// </summary>
    public class ReClassifierController
    {
        /// <summary>
        /// Перепривязываем данные и заносим информацию в журнал замен внутри хранимой процедуры
        /// </summary>
        /// <param name="fromProductionInfo"></param>
        /// <param name="toProductionInfo"></param>
        /// <param name="userId"></param>
        public static void ReClassifier(ProductionInfo fromProductionInfo, ProductionInfo toProductionInfo, Guid userId, DrugClassifierContext context)
        {
            //Значит такой ProductionInfo не было и мы делаем merge на другой
            if (fromProductionInfo == null)
                return;

            if( fromProductionInfo.DrugId == toProductionInfo.DrugId && 
                fromProductionInfo.OwnerTradeMarkId == toProductionInfo.OwnerTradeMarkId &&
                fromProductionInfo.PackerId == toProductionInfo.PackerId)
                return;

            context.Database.CommandTimeout = 6000;

            context.Database.ExecuteSqlCommand(@"[Systematization].[ReplacementProcedure]   @FromDrugId,    @FromOwnerTradeMarkId,  @FromPackerId, @FromProductionInfoId,
                                                                                            @ToDrugId,      @ToOwnerTradeMarkId,    @ToPackerId, @ToProductionInfoId,
                                                                                            @UserId",
                new SqlParameter("@FromDrugId", fromProductionInfo.DrugId),
                new SqlParameter("@FromOwnerTradeMarkId", fromProductionInfo.OwnerTradeMarkId),
                new SqlParameter("@FromPackerId", fromProductionInfo.PackerId),
                new SqlParameter("@FromProductionInfoId", fromProductionInfo.Id),
                new SqlParameter("@ToDrugId", toProductionInfo.DrugId),
                new SqlParameter("@ToOwnerTradeMarkId", toProductionInfo.OwnerTradeMarkId),
                new SqlParameter("@ToPackerId", toProductionInfo.PackerId),
                new SqlParameter("@ToProductionInfoId", toProductionInfo.Id),
                new SqlParameter("@UserId", userId));
           


        }





    }
}
