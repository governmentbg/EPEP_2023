using System.Text.Json.Serialization;

namespace Epep.Core.ViewModels.Case
{
    public class UserWithAccessVM
    {
        public string UserName { get; set; }
        //[JsonIgnore]
        //public string Uic { get; set; }
        //[JsonIgnore]
        //public int UserType { get; set; }
        public string SideName { get; set; }
        //[JsonIgnore]
        //public int SubjectTypeId { get; set; }
        public string SideKindName { get; set; }
    }
}
