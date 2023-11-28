namespace Epep.Core.ViewModels.Case
{
    public class DocumentVM
    {
        public Guid Gid { get; set; }
        public int Direction { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Number { get; set; }
        public DateTime? Date { get; set; }
        public string SubjectName { get; set; }

        public bool HasFiles { get; set; }
    }
}
