using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models

{
    [Table("CodeMappings")]
    public partial class CodeMapping
    {
        [Key]
        public int Id { get; set; }
        public string Alias { get; set; }
        public string InnerCode { get; set; }
        public string OuterCode { get; set; }
        public string Description { get; set; }
    }
}