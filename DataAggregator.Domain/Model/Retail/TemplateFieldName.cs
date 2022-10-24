using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.Retail
{
    public class TemplateFieldName
    {
        [Key]
        public string FieldName { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public virtual IList<TemplateField> TemplateField { get; set; } 
    }
}
