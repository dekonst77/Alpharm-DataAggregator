using System.Collections.Generic;

namespace DataAggregator.Domain.Model.Common
{
    public class HierarchicalDictionaryItem<T>: DictionaryItem
    {
        public long? ParentId { get; set; }

        public virtual T Parent { get; set; }

#warning переименовать в Children
        public virtual ICollection<T> Childs { get; set; }

        public string Description { get; set; }

        public int ValueLevel { get; set; }
    }
}