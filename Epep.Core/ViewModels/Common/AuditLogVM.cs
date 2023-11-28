using Epep.Core.Convertors;
using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Epep.Core.ViewModels.Common
{
    public class AuditLogVM
    {
        public string OperationName { get; set; }
        public string UserFullName { get; set; }
        public string ObjectInfo { get; set; }
        public string ActionInfo { get; set; }
        public DateTime DateWrt { get; set; }
        public string ClientIp { get; set; }
    }

    public class AuditLogFilterVM
    {
        [Display(Name = "Дата от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Дата до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime DateTo { get; set; }

        [Display(Name = "Потребител")]
        public string UserName { get; set; }

        [Display(Name = "Обект")]
        public string Object { get; set; }

        public void UpdateNullables()
        {
            UserName = UserName.EmptyToNull();
            Object = Object.EmptyToNull();
        }
    }
}
