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

        public DrugFilter(string APP, long SourceId, long PeriodId)
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
            query.Append(string.Format(@"   select  c.Id, drugPeriod.DrugClearId 
                                            into #R1
                                            from    Systematization.DrugClear drug with(nolock)
                                                    inner join Systematization.DrugClearPeriod drugPeriod with(nolock) on drugPeriod.DrugClearId = drug.Id
                                                    left join Systematization.DrugClassifier c with(nolock) on drugPeriod.Id = c.DrugClearPeriodId 
                                                    left join Classifier.Drug d with(nolock) on c.DrugId = d.Id
                                                    left join Systematization.DrugClassifierInWork dciw with(nolock) on c.DrugClearPeriodId = dciw.DrugClearPeriodId
                                            where drugPeriod.PeriodId={3} 
		                                        and drug.SourceId={2} 
		                                        and dciw.DrugClearPeriodId is null
                                            ",
               Count, userGuid, SourceId, PeriodId));

            string queryADD = string.Format(@"  
                                                create index ix_r1_data ON #R1 (DrugClearId);

                                                insert into #data(DrugClassifierId)
                                                select  top {0} R.Id 
                                                from    Systematization.DrugClear drug with(nolock)
                                                inner join #R1 R on R.DrugClearId=drug.Id
                                                order by drug.ShortText", 10 * Count);


            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                int inum;
                var cnt = Additional.DrugClearId
                            .Replace(" ", ",")
                            .Split(',')
                            .Where(x => x != string.Empty && int.TryParse(x, out inum))
                            .Select(x => x.Trim()).ToList();

                if (cnt.Count > 0)
                    Additional.DrugClearId = string.Join(",", cnt);
                else
                    return String.Empty;

                query.Append(string.Format(" and drug.Id in ({0});", Additional.DrugClearId));

                query.Append(queryADD);

                return query.ToString();
            }

            if (Additional != null && !string.IsNullOrEmpty(Additional.GZ_code))
            {
                var cnt = Additional.GZ_code
                        .Replace("*", "")
                        .Replace("'", "")
                        .Replace(" ", ",")
                        .Split(',')
                        .Where(x => x != string.Empty)
                        .Select(x => x.Trim()).ToList();

                if (cnt.Count > 0)
                    Additional.GZ_code = string.Join(",", cnt);
                else
                    return String.Empty;

                query.Append(string.Format(@" 
                        and c.Id in (   select PurchaseObjectReady.DrugClassifierId
                                    from    [GovernmentPurchases].dbo.Purchase 
                                                inner join [GovernmentPurchases].dbo.Lot ON Purchase.Id = Lot.PurchaseId 
                                                inner join [GovernmentPurchases].dbo.PurchaseObjectReady ON Lot.Id = PurchaseObjectReady.LotId
                                    where   Purchase.Number in ('{0}')
                                    union
                                    select  ContractObjectReady.DrugClassifierId
                                    from    [GovernmentPurchases].dbo.Contract 
                                           inner join [GovernmentPurchases].dbo.ContractObjectReady ON Contract.Id = ContractObjectReady.ContractId
                                    where   Contract.ReestrNumber in ('{0}')); ", Additional.GZ_code.Replace(",", "','")));

                query.Append(queryADD);

                return query.ToString();
            }

            List<string> whereBlock = new List<string>();

            if (DrugClearWorkStat.IsChecked == true) //Не проставленное
            {
                //Лекарственные средства
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add("(c.DrugId is null AND (c.ForChecking = 0 and c.ForAdding = 0 and c.IsOther = 0))");
                }
                //ДОП
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND (c.ForChecking = 0 and c.ForAdding = 0 and c.IsOther = 1))");
                }
                //ДОП без категории
                if (DataTypeStat.Any(c => c.FullName == "ДОП без категории" && c.IsChecked))
                {
                    whereBlock.Add("(c.GoodsId is null AND c.IsOther = 1 AND c.GoodsCategoryId is null)");
                }
                //Данные на заведение
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add("(c.ForAdding = 1 and c.IsOther = 0)");
                }
                //Данные на проверку
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add("(c.ForChecking = 1 and c.IsOther = 0)");
                }
                //Данные на заведение ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(c.ForAdding = 1 and c.IsOther = 1)");
                }
                //Данные на проверку ДОП
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add("(c.ForChecking = 1 and c.IsOther = 1)");
                }
                if (DataTypeStat.Where(w => w.IsChecked == true).Count() == 0)
                {
                    whereBlock.Add("(c.GoodsId is null and c.DrugId is null)");
                }

            }
            //Для пользователей
            var checkMoreThen5 = UserWorkStat.Where(u => u.IsChecked == true).Count() >= 5;

            if (checkMoreThen5)
            {
                //ЛС
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.DrugId is not null and c.LastChangedUserId is not null"));
                }
                //"Данные на проверку" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForChecking = 1 AND c.ForCheckingUserId is not null"));
                }
                //"Данные на заведение" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForAdding = 1 AND c.ForAddingUserId is not null"));
                }
                //"Данные на заведение ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForAdding = 1 AND c.ForAddingUserId is not null"));
                }
                //"Данные на проверку ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForChecking = 1 AND c.ForCheckingUserId is not null"));
                }
                //"ДОП"
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId is not null"));
                }
            }
            else
            {
                foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
                {
                    //ЛС
                    if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.DrugId is not null and c.LastChangedUserId = '{0}'", user.UserId));
                    }
                    //"Данные на проверку" 
                    else if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                    }
                    //"Данные на заведение" 
                    else if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 0 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                    }
                    //"Данные на заведение ДОП" 
                    else if (DataTypeStat.Any(c => c.FullName == "Данные на заведение ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                    }
                    //"Данные  на проверку ДОП" 
                    else if (DataTypeStat.Any(c => c.FullName == "Данные на проверку ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                    }
                    //"ДОП"
                    else if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                    {
                        whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId = '{0}'", user.UserId));
                    }
                    else //по выбранным пользователям
                        whereBlock.Add(string.Format(@" c.LastChangedUserId = '{0}'", user.UserId));
                }
            }

            if (whereBlock.Count > 0)
            {
                StringBuilder whereQuery = new StringBuilder();
                foreach (var block in whereBlock)
                {
                    if (whereQuery.Length > 0)
                        whereQuery.Append(" OR ");

                    whereQuery.Append(" (" + block + ") ");
                }
                query.Append(query.ToString().Trim().EndsWith("AND") ? "" : "and ( " + whereQuery.ToString() + " ) ");
            }
            //else
            //    return String.Empty;

            var s_PrioritetStat = new StringBuilder();
            foreach (var PS in PrioritetStat.Where(u => u.IsChecked == true))
            {
                if (s_PrioritetStat.Length > 0)
                    s_PrioritetStat.Append(" OR ");

                s_PrioritetStat.Append("( c.Id in (select DrugClassifierId from [Systematization].[PrioritetDrugClassifier] with(nolock) where [isControl]=0 and [PrioritetWordsId]=" + Convert.ToString(PS.PrioritetWordsId) + ")");

                if (PS.isReady)
                    s_PrioritetStat.Append(" AND c.ClassifierId > 0");
                else
                    s_PrioritetStat.Append(" AND c.ClassifierId is null");

                if (PS.IsOther)
                    s_PrioritetStat.Append(" AND c.IsOther=1");
                else
                    s_PrioritetStat.Append(" AND c.IsOther=0");

                s_PrioritetStat.Append(")");
            }

            if (s_PrioritetStat.Length > 0)
                query.Append(query.ToString().Trim().EndsWith("AND") ? "" : "and (" + s_PrioritetStat.ToString() + ")");

            //В обработку доп
            if (CategoryStat != null && CategoryStat.Any(c => c.IsChecked))
            {
                var CategoryStatWhere = new StringBuilder();
                foreach (var cat in CategoryStat.Where(w => w.IsChecked == true))
                {
                    if (CategoryStatWhere.Length > 0)
                        CategoryStatWhere.Append(",");

                    CategoryStatWhere.Append(cat.CategoryId.ToString());
                }

                if (CategoryStatWhere.Length > 0)
                    query.Append(query.ToString().Trim().EndsWith("AND") ? "" : "and c.GoodsCategoryId in (" + CategoryStatWhere.ToString() + ")");
            }

            //Ограничения по дате
            var date_where_in = new List<string>();
            foreach (var date in DateStat.Where(u => u.IsChecked == true))
                date_where_in.Add(string.Format(@"'" + date.date.Replace("-", "") + "'"));

            if (date_where_in.Count > 0)
            {
                if (date_where_in.Count == 1)
                    query.Append(query.ToString().Trim().EndsWith("AND") ? "" : string.Format(@" and drug.[date] <={0}", string.Join(",", date_where_in)));
                else
                    query.Append(query.ToString().Trim().EndsWith("AND") ? "" : string.Format(@" and drug.[date] in({0})", string.Join(",", date_where_in)));
            }

            //Накладываем дополнительный фильтр
            if (Additional != null)
            {
                var additionalWhere = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalWhere))
                    query.Append(query.ToString().Trim().EndsWith("AND") ? "" : " and " + additionalWhere);
            }

            query.Append(GetOrderCondition(userGuid));

            query.Append(queryADD);

            return query.ToString();
        }

        [Obsolete("Method string GetFilter_v1 is deprecated", true)]
        public string GetFilter_v1(Guid userGuid)
        {
            var query = new StringBuilder();

            query.Append(string.Format(@"
                select dr.Id 
                from (  select top({0}) c.Id 
                        from Systematization.DrugClear drug with(nolock)
                            inner join Systematization.DrugClearPeriod drugPeriod with(nolock) on drugPeriod.DrugClearId = drug.Id
                            left join Systematization.DrugClassifier c with(nolock) on drugPeriod.Id = c.DrugClearPeriodId 
                            left join Classifier.Drug d with(nolock) on c.DrugId = d.Id
		                    left join Systematization.DrugClassifierInWork dciw with(nolock) on c.DrugClearPeriodId = dciw.DrugClearPeriodId
                        where drugPeriod.PeriodId={3} 
		                        and drug.SourceId={2} 
		                        and dciw.DrugClearPeriodId is null
                                and ",
               10 * Count, userGuid, SourceId, PeriodId));

            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                int inum;
                var cnt = Additional.DrugClearId
                            .Replace(" ", ",")
                            .Split(',')
                            .Where(x => x != string.Empty && int.TryParse(x, out inum))
                            .Select(x => x.Trim()).ToList();

                if (cnt.Count > 0)
                    Additional.DrugClearId = string.Join(",", cnt);
                else
                    return String.Empty;

                query.Append(string.Format(" drug.Id in ({0})) as dr GROUP BY dr.Id ", Additional.DrugClearId));
                return query.ToString();
            }

            if (Additional != null && !string.IsNullOrEmpty(Additional.GZ_code))
            {
                var cnt = Additional.GZ_code
                        .Replace(" ", ",")
                        .Split(',')
                        .Where(x => x != string.Empty)
                        .Select(x => x.Trim()).ToList();

                if (cnt.Count > 0)
                    Additional.GZ_code = string.Join(",", cnt);
                else
                    return String.Empty;

                query.Append(string.Format(@" 
                        c.Id in (   select PurchaseObjectReady.DrugClassifierId
                                    from    [GovernmentPurchases].dbo.Purchase 
                                                inner join [GovernmentPurchases].dbo.Lot ON Purchase.Id = Lot.PurchaseId 
                                                inner join [GovernmentPurchases].dbo.PurchaseObjectReady ON Lot.Id = PurchaseObjectReady.LotId
                                    where   Purchase.Number in ('{0}')
                                    union
                                    select  ContractObjectReady.DrugClassifierId
                                    from    [GovernmentPurchases].dbo.Contract 
                                           inner join [GovernmentPurchases].dbo.ContractObjectReady ON Contract.Id = ContractObjectReady.ContractId
                                    where   Contract.ReestrNumber in ('{0}'))
                                ) as dr 
                        group by dr.Id ",
                Additional.GZ_code.Replace(",", "','")));

                return query.ToString();
            }

            return query.ToString();
        }

        public string GetFilter(Guid userGuid)
        {

            StringBuilder query = new StringBuilder();

            query.Append(string.Format(@"
                select c.id from (
	                select top({0}) drugPeriod.Id 
                    from Systematization.DrugClear drug with(nolock)
		                inner join Systematization.DrugClearPeriod drugPeriod with(nolock) on drugPeriod.DrugClearId = drug.Id
		                left join Systematization.DrugClassifier c with(nolock) on drugPeriod.Id = c.DrugClearPeriodId 
		                left join Classifier.Drug d with(nolock) on c.DrugId = d.Id
		                left join Systematization.DrugClassifierInWork dciw with(nolock) on c.DrugClearPeriodId = dciw.DrugClearPeriodId
	                where drugPeriod.PeriodId={3} 
		                and drug.SourceId={2} 
		                and dciw.DrugClearPeriodId is null
                        and 
                ",
               Count, userGuid, SourceId, PeriodId));

            //Накладываем дополнительный фильтр
            if (Additional != null && !string.IsNullOrEmpty(Additional.DrugClearId))
            {
                int inum;
                var cnt = Additional.DrugClearId
                            .Replace(" ", ",")
                            .Split(',')
                            .Where(x => x != string.Empty && int.TryParse(x, out inum))
                            .Select(x => x.Trim()).ToList();

                if (cnt.Count > 0)
                    Additional.DrugClearId = string.Join(",", cnt);
                else
                    return String.Empty;

                query.Append(string.Format(" drug.Id in ({0})) as dr GROUP BY dr.Id ", Additional.DrugClearId));

                return query.ToString();
            }

            List<string> whereBlock = new List<string>();

            //Роботы
            if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
            {
                foreach (var robot in RobotStat.Where(r => r.IsChecked))
                {
                    whereBlock.Add(string.Format(" c.RobotId = {0} and c.DrugId is not null and c.OwnerTradeMarkId is not null and c.LastChangedUserId is null ", robot.Id));
                }
            }

            //В обработку ЛС
            if (DrugClearWorkStat.IsChecked && DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
            {
                //#StatusHistory h.StatusId = 100 AND (c.Id is null or (c.ForChecking = 0 and c.ForAdding = 0 and c.IsOther = 0))
                whereBlock.Add(" (c.DrugId is null AND (c.ForChecking = 0 and c.ForAdding = 0 and c.IsOther = 0))");

            }

            //В обработку доп
            if (CategoryStat != null && CategoryStat.Any(c => c.IsChecked))
            {
                var CategoryStatWhere = new StringBuilder();
                foreach (var cat in CategoryStat.Where(w => w.IsChecked == true))
                {
                    if (CategoryStatWhere.Length > 0)
                        CategoryStatWhere.Append(",");

                    CategoryStatWhere.Append(cat.CategoryId.ToString());
                }

                if (CategoryStatWhere.Length > 0)
                    query.Append("and c.GoodsCategoryId in (" + CategoryStatWhere.ToString() + ")");
            }

            //Для пользователей
            foreach (var user in UserWorkStat.Where(u => u.IsChecked == true))
            {
                //ЛС
                if (DataTypeStat.Any(c => c.FullName == "Лекарственные средства" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 1000 AND c.LastChangedUserId = '{0}' AND c.DrugId is not null
                    whereBlock.Add(string.Format(@" c.LastChangedUserId = '{0}' AND c.DrugId is not null", user.UserId));
                }

                //"ДОП" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на проверку" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 100 AND c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'
                    whereBlock.Add(string.Format(@" c.ForChecking = 1 AND c.ForCheckingUserId = '{0}'", user.UserId));
                }

                //"Данные на заведение" 
                if (DataTypeStat.Any(c => c.FullName == "Данные на заведение" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 100 AND c.ForAdding = 1 AND c.ForAddingUserId = '{0}'
                    whereBlock.Add(string.Format(@" c.ForAdding = 1 AND c.ForAddingUserId = '{0}'", user.UserId));
                }

                //"ДОП"
                if (DataTypeStat.Any(c => c.FullName == "ДОП" && c.IsChecked))
                {
                    //#StatusHistory h.StatusId = 1000 AND c.IsOther = 1 AND c.IsOtherUserId = '{0}'
                    whereBlock.Add(string.Format(@" c.IsOther = 1 AND c.IsOtherUserId = '{0}'", user.UserId));
                }
            }

            if (whereBlock.Count > 0)
            {
                StringBuilder whereQuery = new StringBuilder();
                foreach (var block in whereBlock)
                {
                    if (whereQuery.Length > 0)
                        whereQuery.Append(" OR ");

                    whereQuery.Append(" (" + block + ") ");
                }
                query.Append(" ( " + whereQuery.ToString() + " ) ");
            }
            else
                return String.Empty;

            //Ограничения по дате
            var date_where_in = new List<string>();
            foreach (var date in DateStat.Where(u => u.IsChecked == true))
                date_where_in.Add(string.Format(@"'" + date.date.Replace("-", "") + "'"));

            if (date_where_in.Count > 0)
            {
                if (date_where_in.Count == 1)
                    query.Append(string.Format(@" AND drug.[date] <={0}", string.Join(",", date_where_in)));
                else
                    query.Append(string.Format(@" AND drug.[date] in({0})", string.Join(",", date_where_in)));
            }

            //Накладываем дополнительный фильтр
            if (Additional != null)
            {
                var additionalWhere = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalWhere))
                    query.Append(" AND " + additionalWhere);
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
                    throw new ApplicationException("Текущего пользователя нет в системе обработки информации"); ;

                if (userSource.PeriodId == 69) //GZ и контракты (задача 4675)
                    return "ORDER BY drug.Date,c.Id desc;";

                if (userSource.PeriodId == 2 || userSource.PeriodId == 12) //GZ и контракты (задача 4675)
                    return "ORDER BY c.Id desc;";

                return "ORDER BY drug.ShortText;";
            }
        }

        [Obsolete("Method IList<DrugClear> GetDrugs is deprecated", true)]
        public IList<DrugClear> GetDrugs(DrugClassifierContext context)
        {

            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@" SELECT TOP({0}) drug.* 
                    FROM  Systematization.DrugClear as drug 
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
                        block2.Append("(c.Id is null or c.ForChecking = 0 and c.ForAdding = 0 and c.IsOther = 0)");
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
