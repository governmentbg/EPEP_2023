using Newtonsoft.Json.Converters;

namespace Epep.Core.Convertors
{
    public class BgDateConvertor : IsoDateTimeConverter
    {
        public BgDateConvertor()
        {
            DateTimeFormat = "dd.MM.yyyy";
        }

        public BgDateConvertor(string format)
        {
            DateTimeFormat = format;
        }
    }
   
}
