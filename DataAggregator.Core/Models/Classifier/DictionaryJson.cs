using System;
using DataAggregator.Domain.Model.Common;

namespace DataAggregator.Core.Models.Classifier
{
    public class DictionaryJson
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Key { get; set; }

        public DictionaryJson()
        {
            
        }

        public DictionaryJson(DictionaryItem dictionaryItem)
        {
            Id = 0;
            Value = String.Empty;

            if (dictionaryItem != null)
            {
                Id = dictionaryItem.Id;
                Key = dictionaryItem.Id.ToString();
                Value = dictionaryItem.Value;
            }
        }
    }    
}