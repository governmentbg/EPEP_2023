namespace Epep.Core.ViewModels.Case
{
    public class SideVM
    {
        public Guid Gid { get; set; }
        public string SubjectName { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SideInvolvementKindName { get; set; }

        public string[] Lawyers { get; set; }

        public string ProceduralRelation { get; set; }
    }


}
