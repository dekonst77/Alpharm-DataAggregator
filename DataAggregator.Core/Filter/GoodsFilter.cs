using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAggregator.Core.Filter
{
    public class GoodsFilter
    {
        public int Count { get; set; }

        public List<long?> ForWorkCategoryIds { get; set; }

        public List<long?> ForAddingCategoryIds { get; set; }

        public List<string> UserGuids { get; set; }

        public AdditionalGoodsFilter Additional { get; set; }
        
        public GoodsFilter()
        {
            // значения по умолчанию
            Count = 10;
        }

        public string GetFilter()
        {
            var query = new StringBuilder();

            query.AppendFormat(@"
                    select top {0} c.GoodsClearId
                    from GoodsSystematization.GoodsClassifier as c
                        inner join GoodsSystematization.GoodsClear as gc on gc.Id = c.GoodsClearId
                        left join GoodsSystematization.GoodsClassifierInWork as w on w.GoodsClearId = c.GoodsClearId
                        left join Systematization.DrugClear as dc on dc.Id = gc.DrugClearId
                    where w.GoodsClearId is null", Count);

            string forWorkSqlPart = String.Empty;

            // данные "в работу"
            if (ForWorkCategoryIds.Count > 0)
            {
                var conditionsList = ForWorkCategoryIds.Where(id => id != null).Select(id => string.Format("gc.GoodsCategoryId = {0}", id)).ToList();

                if (ForWorkCategoryIds.Any(id => id == null))
                {
                    conditionsList.Add("gc.GoodsCategoryId is null");
                }

                forWorkSqlPart = string.Format("(c.ForAdding = 0 and ({0}))", string.Join(" or ", conditionsList));
            }

            string forAddingSqlPart = String.Empty;

            // данные "на заведение"
            if (ForAddingCategoryIds.Count > 0)
            {
                var conditionsList = ForAddingCategoryIds.Where(id => id != null).Select(id => string.Format("gc.GoodsCategoryId = {0}", id)).ToList();

                if (ForAddingCategoryIds.Any(id => id == null))
                {
                    conditionsList.Add("gc.GoodsCategoryId is null");
                }

                forAddingSqlPart = string.Format("(c.ForAdding = 1 and ({0}))", string.Join(" or ", conditionsList));
            }

            if (!string.IsNullOrEmpty(forWorkSqlPart) || !string.IsNullOrEmpty(forAddingSqlPart))
            {
                query.Append(string.Format(" and ({0}{1}{2})",
                    forWorkSqlPart,
                    !string.IsNullOrEmpty(forWorkSqlPart) && !string.IsNullOrEmpty(forAddingSqlPart) ? " or " : String.Empty,
                    forAddingSqlPart)
                );                
            }

            // готовые или нет
            if (UserGuids.Count > 0)
            {
                var conditionsList = UserGuids.Where(id => id != null).Select(id => string.Format("c.LastChangedUserId = '{0}'", id)).ToList();

                if (UserGuids.Any(id => id == null))
                {
                    conditionsList.Add("c.LastChangedUserId is null");
                }

                query.Append(string.Format(" and ({0})", string.Join(" or ", conditionsList)));                
            }
            else
            {
                query.Append(" and c.GoodsId is null");
            }

            // Накладываем дополнительный фильтр
            if (Additional != null)
            {
                string additionalCondition = Additional.GetFilter();

                if (!string.IsNullOrEmpty(additionalCondition))
                {
                    query.Append(" and ");
                    query.Append(additionalCondition);
                }
            }

            query.Append(" order by dc.Text");

            return query.ToString();
        }

       
    }
}
