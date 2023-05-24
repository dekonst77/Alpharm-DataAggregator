namespace DataAggregator.Domain.Model.Common
{
    public class DictionaryItem
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
    public class SPRItem
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsUse { get; set; }
    }
    public class DictionaryItem_t1
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Value_Eng { get; set; }
    }

    public class DictionaryItem_t4
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Value_Eng { get; set; }

        public bool UseClassifier { get; set; }

        public bool UseGoodsClassifier { get; set; }
    }

    /// <summary>
    /// справочник <Форма выпуска> для доп. материалов
    /// </summary>
    public class DictionaryItem_t5
    {
        public long Id { get; set; }
        public long GoodsTradeNameId { get; set; }
        public string GoodsTradeName { get; set; }
        public string MiniName { get; set; }
        public string GoodsDescription { get; set; }
        public string GoodsDescription_Eng { get; set; }
    }

    /// <summary>
    /// справочник ДООП -> Категории
    /// </summary>
    public class DictionaryItem_GoodsCategory
    {
        public long Id { get; set; }
        public long GoodsSectionId { get; set; }
        public string GoodsSectionName { get; set; }
        public string MiniName { get; set; }
        public string Value { get; set; } // категория
        public string Value_Eng { get; set; } // категория перевод на English
    }
    public class SPRItem_t2
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Description_Eng { get; set; }
        public bool IsUse { get; set; }
    }
    public class SPRItem_t_return
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Value_Eng { get; set; }
        public string MiniName { get; set; }        
        public string Description { get; set; }
        public string Description_Eng { get; set; }
        public bool IsUse { get; set; }
        public string GoodsDescription { get; set; }
        public string GoodsDescription_Eng { get; set; }
    }
    public class SPRItem_t3
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
