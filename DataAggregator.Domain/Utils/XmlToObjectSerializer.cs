using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DataAggregator.Domain.Utils
{
    /// <summary>
    /// Сериализует объект в xml
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class XmlToObjectSerializer<T> where T : class
    {
        public static string Serialize(T obj)
        {
            // Remove Declaration  
            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj);
                    return stream.ToString();
                }
            }
        }
    }
}
