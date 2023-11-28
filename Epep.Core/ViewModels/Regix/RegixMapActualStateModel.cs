using Newtonsoft.Json;

namespace Epep.Core.Services.ViewModels.Regix
{
    public class RegixMapActualStateModel : IO.RegixClient.Contracts.IRegixMapTRActualState
    {
        [JsonProperty("field_ident")]
        public string FieldIdent { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }

        [JsonProperty("labels")]
        public string Labels { get; set; }

        /// <summary>
        /// field_object = Обект, field_code - Код
        /// </summary>
        [JsonProperty("type_field")]
        public string TypeField { get; set; }

        /// <summary>
        /// Този код дали да се показва в справката
        /// </summary>
        [JsonProperty("for_display")]
        public bool? ForDisplay { get; set; }

        /// <summary>
        /// Има ли обект в кода за да се замени с полетата му
        /// </summary>
        [JsonProperty("has_object")]
        public bool? HasObject { get; set; }
    }
}
