using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class Template
    {
        public long Id { get; set; }

        public long SourceId { get; set; }

        public bool IsActual { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Количество строк, в которых описан заголовок
        /// </summary>
        public int NumberHeaderRows { get; set; }

        [JsonIgnore]
        public virtual Source Source { get; set; }
        [JsonIgnore]
        public virtual IList<TemplateField> TemplateField { get; set; }
        [JsonIgnore]
        public virtual IList<FileData> FileData { get; set; } 
    }
}
