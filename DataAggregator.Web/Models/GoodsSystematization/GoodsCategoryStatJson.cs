using DataAggregator.Domain.Model.DrugClassifier.Stat;

namespace DataAggregator.Web.Models.GoodsSystematization
{
    public class GoodsCategoryStatJson : GoodsCategoryStat
    {
        public string SectionName { get; set; }

        public GoodsCategoryStatJson(GoodsCategoryStat goodsCategoryStat)
        {
            Id = goodsCategoryStat.Id;
            CategoryId = goodsCategoryStat.CategoryId;
            ForAdding = goodsCategoryStat.ForAdding;
            CategoryName = goodsCategoryStat.CategoryName;
            ForWorkCount = goodsCategoryStat.ForWorkCount;
            InWorkCount = goodsCategoryStat.InWorkCount;
            IsReadyCount = goodsCategoryStat.IsReadyCount;
            if (goodsCategoryStat.GoodsCategory != null)
            {
                SectionName = goodsCategoryStat.GoodsCategory.GoodsSection.Name;
            }
        }
    }
}