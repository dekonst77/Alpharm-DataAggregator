using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Stat;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;

namespace DataAggregator.Core.Filter
{
    public class DrugFilter
    {
        public int Count { get; set; }

        public List<RobotStat> RobotStat { get; set; }
        public List<DateStat> DateStat { get; set; } 
        public DrugClearWorkStat DrugClearWorkStat { get; set; }

        public List<UserWorkStat> UserWorkStat { get; set; }
        public List<PrioritetStat> PrioritetStat { get; set; }

        public List<DataTypeStat> DataTypeStat { get; set; }
        public List<CategoryStatDrugView> CategoryStat { get; set; }
        public AdditionalFilter Additional { get; set; }

        public string APP = "DrugFilter";
        public long SourceId = 0;
        public long PeriodId = 0;
        public DrugFilter(string APP,long SourceId,long PeriodId)
        {
            this.SourceId = SourceId;
            this.PeriodId = PeriodId;
            this.APP = APP;
            // значения по умолчанию
            Count = 10;
        }
        public string GetFilter_v2(Guid userGuid)
        {

            StringBuilder query = new StringBuilder();


            query.Append(string.Format(@"   SELECT  c.Id,DrugClearId into #R1
                                            FROM    Systematization.DrugClear(nolock) as drug 
                                                    INNER JOIN Systematization.DrugClearPeriod(nolock) as drugPeriod ON drugPeriod.DrugClearId = drug.Id AND drugPeriod.PeriodId={3} AND drug.SourceId={2}
                                                    LEFT JOIN Systematization.DrugClassifier(nolock) as c ON drugPeriod.Id = c.DrugClearPeriodId 
                                                    LEFT JOIN Classifier.Drug(nolock) as d on c.DrugId = d.Id
                                            WHERE 
                                                    c.DrugClearPeriodId not in(select DrugClearPeriodId from [Systematization].[DrugClassifierInWork](nolock))
                                                    AND 
                                            ",
               Count,
               userGuid, SourceId, PeriodId));

            string queryADD = string.Format(@"  INSERT INTO #data(DrugClassifierId)
                                                SELECT  top {0} R.Id 
                                                FROM    Systematization.DrugClear(nolock) as drug 
                                                        INNER JOIN #R1 R on R.DrugClearId=drug.Id
                                                ORDER BY drug.ShortText", 10*Count);




            //Накладываем дополнительный фильтр
            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                query.Append(string.Format(" drug.Id in ({0}) or c.id in ({0})) as dr GROUP BY dr.Id ", Additional.DrugClearId));
                return query.ToString();
            }
            if (Additional != null && !string.IsNullOrEmpty(Additional.GZ_code))
            {
                query.Append(string.Format(@"   c.Id in(
                                                SELECT       PurchaseObjectReady.DrugClassifierId
                                                FROM[GovernmentPurchases]..Purchase INNER JOIN
                                                                         [GovernmentPurchases]..Lot ON Purchase.Id = Lot.PurchaseId INNER JOIN
                                                                         [GovernmentPurchases]..PurchaseObjectReady ON Lot.Id = PurchaseObjectReady.LotId
                                                where Purchase.Number in ('{0}')
                                                union
                                                SELECT       ContractObjectReady.DrugClassifierId
                                                FROM[GovernmentPurchases]..Contract INNER JOIN
                                                                         [GovernmentPurchases]..ContractObjectReady ON Contract.Id = ContractObjectReady.ContractId
                                                where Contract.ReestrNumber in ('{0}'))
                                                ) as dr GROUP BY dr.Id ", 
                    Additional.GZ_code.Replace(" ", "").Replace(",", "','")));
                return query.ToString();
            }

            List<string> whereBlock = new List<string>();
           
            if (DrugClearWorkStat.IsChecked == true)//Не проставленное
            {
                //Лекарственные средства
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add("(c.DrugId is null AND (c.ForChecking = 0 and ForAdding = 0 and IsOther = 0))");
                }
                //ДОП
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND (c.ForChecking = 0 and ForAdding = 0 and IsOther = 1))");
                }
                //ДОП без категории
                if (DataTypeStat.Any(c => c.FullName == "ДОП без категории" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND IsOther = 1 AND GoodsCategoryId is null)");
                }
                //Данные на заведение
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add("(ForAdding = 1 and IsOther = 0)");
                }
                //Данные на проверку
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add("(ForChecking = 1 and IsOther = 0)");
                }
                //Данные на заведение ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(ForAdding = 1 and IsOther = 1)");
                }
                //Данные на проверку ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(ForChecking = 1 and IsOther = 1)");
                }
                if (DataTypeStat.Where(w => w.IsChecked == true).Count() == 0)
                {
                    whereBlock.Add("(GoodsId is null and DrugId is null)");
                }

            }
            //Для пользователей
            if (UserWorkStat.Where(u => u.IsChecked == true).Count() >= 5)
            {
                //ЛС
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.LastChangedUserId  is not null AND c.DrugId is not null"));
                }
                //"Данные на проверку" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForChecking = 1 AND c.ForCheckingUserId  is not null"));
                }
                //"Данные на заведение" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForAdding = 1 AND c.ForAddingUserId  is not null"));
                }
                //"Данные на заведение ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForAdding = 1 AND c.ForAddingUserId  is not null"));
                }
                //"Данные на проверку ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForChecking = 1 AND c.ForCheckingUserId is not null"));
                }
                //"ДОП"
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId  is not null"));
                }
            }
            else
            {
                foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
                {
                    //ЛС
                    if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.LastChangedUserId = '{0}' AND c.DrugId is not null", user.UserId));
                    }
                    //"Данные на проверку" 
                    if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                    }
                    //"Данные на заведение" 
                    if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                    }
                    //"Данные на заведение ДОП" 
                    if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                    }
                    //"Данные на проверку ДОП" 
                    if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                    }
                    //"ДОП"
                    if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId = '{0}'", user.UserId));
                    }
                }
            }
            string s_PrioritetStat = "";
            foreach (var PS in PrioritetStat.Where(u => u.IsChecked == true))
            {
                if (s_PrioritetStat != "")
                    s_PrioritetStat += " OR ";
                s_PrioritetStat += "( c.Id in (select DrugClassifierId from [Systematization].[PrioritetDrugClassifier] where [isControl]=0 and [PrioritetWordsId]=" + Convert.ToString(PS.PrioritetWordsId) + ")";

                if (PS.isReady)
                    s_PrioritetStat += " AND c.ClassifierId>0";
                else
                    s_PrioritetStat += " AND c.ClassifierId is NULL";

                if (PS.IsOther)
                    s_PrioritetStat += " AND c.IsOther=1";
                else
                    s_PrioritetStat += " AND c.IsOther=0";

                s_PrioritetStat += ")";
            }
            if (s_PrioritetStat != "")
            {
                s_PrioritetStat = "(" + s_PrioritetStat + ")";
                whereBlock.Add(s_PrioritetStat);
            }

            StringBuilder whereQuery = new StringBuilder();

            if (whereBlock.Count > 0)
            {

                foreach (var block in whereBlock)
                {
                    if (whereQuery.Length > 0)
                        whereQuery.Append(" OR ");
                    whereQuery.Append(" (");
                    whereQuery.Append(block);
                    whereQuery.Append(") ");
                }
            }

            else
            {
                //Данные условия выбора не допустимы - ничего искать не будем
                return String.Empty;
            }

            query.Append(" ( ");
            query.Append(whereQuery);
            query.Append(" ) ");
            //В обработку доп
            if (CategoryStat != null && CategoryStat.Any(c => c.IsChecked))
            {
                var CategoryStatWhere = "";
                foreach (var cat in CategoryStat.Where(w => w.IsChecked == true))
                {
                    if (CategoryStatWhere != "")
                        CategoryStatWhere += ",";
                    CategoryStatWhere += Convert.ToString(cat.CategoryId);

                }
                if (!string.IsNullOrEmpty(CategoryStatWhere))
                {
                    CategoryStatWhere = "[GoodsCategoryId] in(" + CategoryStatWhere + ")";
                    query.Append(" AND ");
                    query.Append(CategoryStatWhere);
                }
            }
            //Ограничения по дате
            string date_where_in = "";
            int date_where_count = 0;
            foreach (var date in DateStat.Where(u => u.IsChecked == true))
            {
                if (date_where_in != "") date_where_in += ",";
                date_where_in += string.Format(@"'" + date.date.Replace("-", "") + "'");
                date_where_count++;
            }


            if (!string.IsNullOrEmpty(date_where_in))
            {
                if (date_where_count == 1)
                    query.Append(string.Format(@" AND drug.[date] <={0}", date_where_in));
                else
                    query.Append(string.Format(@" AND drug.[date] in({0})", date_where_in));
            }

            //Накладываем дополнительный фильтр
            if (Additional != null)
            {
                var additionalWhere = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalWhere))
                {
                    query.Append(" AND ");
                    query.Append(additionalWhere);
                }
            }

            query.Append(GetOrderCondition(userGuid));

            query.Append(@"
                "+queryADD);

            return query.ToString();
        }
        public string GetFilter_v1(Guid userGuid)
        {

            StringBuilder query = new StringBuilder();

            query.Append(string.Format(@"
                SELECT dr.Id 
                FROM (  SELECT TOP({0}) c.Id 
                        FROM Systematization.DrugClear(nolock) as drug 
                             INNER JOIN Systematization.DrugClearPeriod(nolock) AS drugPeriod ON drugPeriod.DrugClearId = drug.Id AND drugPeriod.PeriodId={3} AND drug.SourceId={2}
                             LEFT JOIN Systematization.DrugClassifier(nolock) as c ON drugPeriod.Id = c.DrugClearPeriodId 
                             LEFT JOIN Classifier.Drug(nolock) as d on c.DrugId = d.Id
                             WHERE c.Id not in (SELECT Id from [Systematization].[DrugClassifierInWork](nolock))
                             AND ",
               10*Count, userGuid, SourceId, PeriodId));




            //Накладываем дополнительный фильтр
            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                query.Append(string.Format(" drug.Id in ({0})) as dr GROUP BY dr.Id ", Additional.DrugClearId));
                return query.ToString();
            }
            string s_PrioritetStat = "";
            foreach (var PS in PrioritetStat.Where(u => u.IsChecked == true))
            {
                if (s_PrioritetStat != "")
                    s_PrioritetStat += " OR ";
                s_PrioritetStat += "( c.Id in (select DrugClassifierId from [Systematization].[PrioritetDrugClassifier] where [isControl]=0 and [PrioritetWordsId]=" + Convert.ToString(PS.PrioritetWordsId) + ")";

                if (PS.isReady)
                    s_PrioritetStat += " AND c.ClassifierId>0";
                else
                    s_PrioritetStat += " AND c.ClassifierId is NULL";
                
                if (PS.IsOther)
                    s_PrioritetStat += " AND c.IsOther=1";
                else
                    s_PrioritetStat += " AND c.IsOther=0";

                s_PrioritetStat += ")";
            }
            if (s_PrioritetStat != "")
            {
                s_PrioritetStat = "(" + s_PrioritetStat + ")";
                query.Append(s_PrioritetStat);
            }


            if (Additional != null && !string.IsNullOrEmpty(Additional.GZ_code))
            {
                query.Append(string.Format(@" 
                        c.Id IN (   SELECT PurchaseObjectReady.DrugClassifierId
                                    FROM    [GovernmentPurchases].dbo.Purchase 
                                            INNER JOIN [GovernmentPurchases].dbo.Lot ON Purchase.Id = Lot.PurchaseId 
                                            INNER JOIN [GovernmentPurchases].dbo.PurchaseObjectReady ON Lot.Id = PurchaseObjectReady.LotId
                                    WHERE   Purchase.Number in ('{0}')
                                    UNION
                                    SELECT  ContractObjectReady.DrugClassifierId
                                    FROM    [GovernmentPurchases].dbo.Contract 
                                            INNER JOIN [GovernmentPurchases].dbo.ContractObjectReady ON Contract.Id = ContractObjectReady.ContractId
                                    WHERE   Contract.ReestrNumber in ('{0}'))
                                ) AS dr 
                        GROUP BY dr.Id ", 
                Additional.GZ_code.Replace(" ", "").Replace(",", "','")));

                return query.ToString();
            }

            List<string> whereBlock = new List<string>();
           
            if (DrugClearWorkStat.IsChecked == true)//Не проставленное
            {
                //Лекарственные средства
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add("(c.DrugId is null AND (c.ForChecking = 0 and ForAdding = 0 and IsOther = 0))");
                }
                //ДОП
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND (c.ForChecking = 0 and ForAdding = 0 and IsOther = 1))");
                }
                //ДОП без категории
                if (DataTypeStat.Any(c => c.FullName == "ДОП без категории" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND IsOther = 1 AND GoodsCategoryId is null)");
                }
                //Данные на заведение
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add("(ForAdding = 1 and IsOther = 0)");
                }
                //Данные на проверку
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add("(ForChecking = 1 and IsOther = 0)");
                }
                //Данные на заведение ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(ForAdding = 1 and IsOther = 1)");
                }
                //Данные на проверку ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(ForChecking = 1 and IsOther = 1)");
                }
                if (DataTypeStat.Where(w => w.IsChecked == true).Count() == 0)
                {
                    whereBlock.Add("(GoodsId is null and DrugId is null)");
                }

            }
            //Для пользователей
            foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
            {
                //ЛС
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.LastChangedUserId = '{0}' AND c.DrugId is not null",user.UserId));
                }
                //"Данные на проверку" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                }
                //"Данные на заведение" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                }
                //"Данные на заведение ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                }
                //"Данные на проверку ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                }
                //"ДОП"
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId = '{0}'",user.UserId));
                }

            }

            StringBuilder whereQuery = new StringBuilder();

            if (whereBlock.Count > 0)
            {

                foreach (var block in whereBlock)
                {
                    if (whereQuery.Length > 0)
                        whereQuery.Append(" OR ");
                    whereQuery.Append(" (");
                    whereQuery.Append(block);
                    whereQuery.Append(") ");
                }
            }

            else
            {
                //Данные условия выбора не допустимы - ничего искать не будем
                return String.Empty;
            }

            query.Append(" ( ");
            query.Append(whereQuery);
            query.Append(" ) ");
            //В обработку доп
            if (CategoryStat != null && CategoryStat.Any(c => c.IsChecked))
            {
                var CategoryStatWhere = "";
                foreach (var cat in CategoryStat.Where(w => w.IsChecked == true))
                {
                    if (CategoryStatWhere != "")
                        CategoryStatWhere += ",";
                    CategoryStatWhere += Convert.ToString(cat.CategoryId);

                }
                if (!string.IsNullOrEmpty(CategoryStatWhere))
                {
                    CategoryStatWhere = "[GoodsCategoryId] in(" + CategoryStatWhere + ")";
                    query.Append(" AND ");
                    query.Append(CategoryStatWhere);
                }
            }

            
            //Ограничения по дате
            string date_where_in = "";
            int date_where_count = 0;
            foreach (var date in DateStat.Where(u => u.IsChecked == true))
            {
                if (date_where_in != "") date_where_in += ",";
                date_where_in += string.Format(@"'" + date.date.Replace("-", "") + "'");
                date_where_count++;
            }

            foreach (var date in DateStat.Where(u => u.IsChecked == true))
            {
                if (date_where_in != "") date_where_in += ",";
                date_where_in += string.Format(@"'" + date.date.Replace("-", "") + "'");
                date_where_count++;
            }


            if (!string.IsNullOrEmpty(date_where_in))
            {
                if (date_where_count == 1)
                    query.Append(string.Format(@" AND drug.[date] <={0}", date_where_in));
                else
                    query.Append(string.Format(@" AND drug.[date] in({0})", date_where_in));
            }

            //Накладываем дополнительный фильтр
            if (Additional != null)
            {
                var additionalWhere = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalWhere))
                {
                    query.Append(" AND ");
                    query.Append(additionalWhere);
                }
            }

            query.Append(GetOrderCondition(userGuid));

            query.Append(" ) as dr GROUP BY c.Id ");

            return query.ToString();
        }
        public string GetFilter(Guid userGuid)
        {

            StringBuilder query = new StringBuilder();

            query.Append(string.Format(@"SELECT c.Id FROM (
                SELECT TOP({0}) drugPeriod.Id FROM Systematization.DrugClear as drug 
                INNER JOIN Systematization.DrugClearPeriod as drugPeriod ON drugPeriod.DrugClearId = drug.Id AND drugPeriod.PeriodId={3} AND drug.SourceId={2}
                LEFT JOIN Systematization.DrugClassifier as c ON drugPeriod.Id = c.DrugClearPeriodId 
                LEFT JOIN Classifier.Drug as d on c.DrugId = d.Id
                WHERE 
                c.DrugClearPeriodId not in(select DrugClearPeriodId from [Systematization].[DrugClassifierInWork])
                AND 
                ",
               Count,
               userGuid, SourceId, PeriodId));




            //Накладываем дополнительный фильтр
            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                //#StatusHistory h.StatusId != 500 AND
                query.Append(string.Format(" drug.Id in ({0})) as dr GROUP BY dr.Id ", Additional.DrugClearId));

                return query.ToString();
            }
            
            List<string> whereBlock = new List<string>();

            //Роботы
            if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
            {
                foreach (var robot in RobotStat.Where(r => r.IsChecked))
                {
                    whereBlock.Add(string.Format("c.RobotId = {0} and c.DrugId is not null and c.OwnerTradeMarkId is not null and c.LastChangedUserId is null ", robot.Id));
                }
            }
            //В обработку ЛС
            if (DrugClearWorkStat.IsChecked && DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
            {
                //#StatusHistory h.StatusId = 100 AND (c.Id is null or (c.ForChecking = 0 and ForAdding = 0 and IsOther = 0))
                whereBlock.Add(" (c.DrugId is null AND (c.ForChecking = 0 and ForAdding = 0 and IsOther = 0))");

            }
            //В обработку доп
            if (CategoryStat != null && CategoryStat.Any(c=>c.IsChecked))
            {
                var CategoryStatWhere = "";
                foreach (var cat in CategoryStat.Where(w=>w.IsChecked==true))
                {
                        if (CategoryStatWhere != "")
                            CategoryStatWhere += ",";
                        CategoryStatWhere += Convert.ToString(cat.Id);

                }
                if (!string.IsNullOrEmpty(CategoryStatWhere))
                {
                    CategoryStatWhere = "[GoodsCategoryId] in(" + CategoryStatWhere + ")";
                    query.Append(" AND ");
                    query.Append(CategoryStatWhere);
                }
            }
            //Для пользователей
            foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
            {
                //ЛС
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 1000 AND c.LastChangedUserId = '{0}' AND c.DrugId is not null
                    whereBlock.Add(
                        string.Format(@" c.LastChangedUserId = '{0}' AND c.DrugId is not null",
                            user.UserId));
                }

                //"ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 100 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'
                    whereBlock.Add(string.Format(
                        @" c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                }

                //"Данные на заведение" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 100 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'
                    whereBlock.Add(string.Format(@" c.ForAdding = 1 AND c.ForAddingUserId = '{0}'",
                        user.UserId));
                }

                //"ДОП"
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 1000 AND c.IsOther = 1 AND c.IsOtherUserId = '{0}'
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId = '{0}'",
                        user.UserId));
                }

            }

            StringBuilder whereQuery = new StringBuilder();

            if (whereBlock.Count > 0)
            {

                foreach (var block in whereBlock)
                {
                    if (whereQuery.Length > 0)
                        whereQuery.Append(" OR ");
                    whereQuery.Append(" (");
                    whereQuery.Append(block);
                    whereQuery.Append(") ");
                }
            }

            else
            {
                //Данные условия выбора не допустимы - ничего искать не будем
                return String.Empty;
            }

            query.Append(" ( ");
            query.Append(whereQuery);
            query.Append(" ) ");

            //Ограничения по дате
            string date_where_in = "";
            int date_where_count = 0;
            foreach (var date in DateStat.Where(u => u.IsChecked == true))
            {
                if (date_where_in != "") date_where_in += ",";
                date_where_in += string.Format(@"'" + date.date.Replace("-", "") + "'");
                date_where_count++;
            }


            if (!string.IsNullOrEmpty(date_where_in))
            {
                if (date_where_count == 1)
                    query.Append(string.Format(@" AND drug.[date] <={0}", date_where_in));
                else
                    query.Append(string.Format(@" AND drug.[date] in({0})", date_where_in));
            }

            //Накладываем дополнительный фильтр
            if (Additional != null)
            {
                var additionalWhere = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalWhere))
                {
                    query.Append(" AND ");
                    query.Append(additionalWhere);
                }
            }

            query.Append(GetOrderCondition(userGuid));
            
            query.Append(" ) as dr GROUP BY c.Id ");
            
            return query.ToString();
        }

        private string GetOrderCondition(Guid userGuid)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var userSource = context.UserSource.FirstOrDefault(us => us.UserId == userGuid);
                if (userSource == null)
                {
                    throw new ApplicationException("no UserSource for current user found");
                }

                if (userSource.PeriodId == 69) //GZ и контракты (задача 4675)
                {
                    return "ORDER BY drug.Date,c.Id desc";
                }

                if (userSource.PeriodId == 2 || userSource.PeriodId == 12) //GZ и контракты (задача 4675)
                {
                    return "ORDER BY c.Id desc";
                }

                return "ORDER BY drug.ShortText";
            }
        }

        public IList<DrugClear> GetDrugs(DrugClassifierContext context)
        {

            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@" SELECT TOP({0}) drug.* FROM   Systematization.DrugClear as drug 
LEFT JOIN Systematization.DrugClassifier as c ON drug.Id = c.DrugClearId WHERE ", Count));


            StringBuilder block1 = new StringBuilder();
            block1.Append("(");

            if (DrugClearWorkStat.IsChecked)
            {
                //#StatusHistory - (h.StatusId = 100)
                block1.Append("(c.DrugId is Null and c.IsOther=0)");
            }

            foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
            {
                //#StatusHistory (UserId = '{0}' and h.StatusId = 1000)
                if (block1.Length > 1)
                    block1.Append(" or ");
                block1.Append(string.Format("(UserId = '{0}' AND (c.DrugId is not Null or c.IsOther=1))", user.UserId));
            }

            block1.Append(")");

            query.Append(block1.ToString());
            query.Append(" and ");

            StringBuilder block2 = new StringBuilder();

            block2.Append("(");

            foreach (var dataType in DataTypeStat.Where(d => d.IsChecked == true))
            {
                if (block2.Length > 1)
                    block2.Append(" or ");
                switch (dataType.FullName)
                {
                    case "Лекарственные средства":
                        block2.Append("(c.Id is null or c.ForChecking = 0 and ForAdding = 0 and IsOther = 0)");
                        break;
                    case "Данные на проверку":
                        block2.Append("(c.ForChecking = 1)");
                        break;
                    case "Данные на заведение":
                        block2.Append("(c.ForAdding = 1)");
                        break;
                    case "ДОП":
                        block2.Append("(c.IsOther = 1)");
                        break;
                        
                }
            }

            block2.Append(")");
            query.Append(block2.ToString());
            query.Append(" ORDER BY drug.Text");

            return context.DrugClear.SqlQuery(query.ToString()).ToList();
        }

        
    }
}
