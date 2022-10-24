using System.Collections.Generic;
using System.Xml.Linq;

namespace DataAggregator.Domain.Utils
{
    internal static class ProcedureHelper
    {
        /// <summary>
        /// Получает список элементов в виде XML для передачи в хранимую процедуру
        /// </summary>
        internal static string GetXmlRows<T>(IEnumerable<T> rows)
        {
            var xElement = new XElement("rows");

            foreach (T row in rows)
                xElement.Add(new XElement("row", new XAttribute("value", row)));

            return xElement.ToString();
        }
    }
}