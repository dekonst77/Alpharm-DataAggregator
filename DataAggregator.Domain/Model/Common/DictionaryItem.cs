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
        public string Description { get; set; }
        public string Description_Eng { get; set; }
        public bool IsUse { get; set; }
    }
    public class SPRItem_t3
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
