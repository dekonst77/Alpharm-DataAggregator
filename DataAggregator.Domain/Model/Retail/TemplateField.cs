using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class TemplateField
    {
        public long Id { get; set; }

        public long? TemplateId { get; set; }

        public string FieldName { get; set; }

        public string ColumnNameInFile { get; set; }

        public long? ParentId { get; set; }

        public int OrderNumber { get; set; }

        [JsonIgnore]
        public virtual TemplateFieldName TemplateFieldName { get; set; }

        [JsonIgnore]
        public virtual Template Template { get; set; }

        [JsonIgnore]
        public virtual TemplateField Parent { get; set; }

        public virtual ICollection<TemplateField> Childs { get; set; }

        /// <summary>
        /// Кол-во уровней вложенности = число родитилей + 1
        /// </summary>
        /// <returns></returns>
        public int GetLevelsCount()
        {
            if (Parent == null)
            {
                return 1;
            }
            else
            {
                return Parent.GetLevelsCount() + 1;
            }
        }
    }
}
