namespace Epep.Core.ViewModels.Admin
{
    public class PricelistVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsActive { get; set; }
        public string[] DocumentsList { get; set; }
    }

    public class PricelistEditVM
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string DocumentsIds { get; set; }
        public string DocumentsList { get; set; }
    }
}
