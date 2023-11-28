using System.ComponentModel.DataAnnotations;

namespace Epep.Core.Models
{
    public partial class ElectronicDocumentType
    {

        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int DocumentKind { get; set; }
        public bool IsActive { get; set; }
    }


}
