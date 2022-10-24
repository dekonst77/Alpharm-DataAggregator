using Newtonsoft.Json.Converters;

namespace DataAggregator.Domain.Utils
{
    public class CustomDateTimeConverterWithTime : IsoDateTimeConverter
    {
        public CustomDateTimeConverterWithTime()
        {
            base.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        }
    }
}
