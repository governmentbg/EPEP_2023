namespace Epep.Core.ViewModels
{
    public class GidLoaderVM
    {
        public Guid Gid { get; set; }
        public Guid? ParentGid { get; set; }
        public Guid? ObjectGid { get; set; }
        public bool Public { get; set; }
        public bool InternalCall { get; set; }
    }
}
