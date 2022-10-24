using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    /// <summary>
    /// Локализация
    /// </summary>
    [Table("Localization", Schema = "Classifier")]
    public class Localization : Common.DictionaryItem
    {
    }
}
