namespace eCase.Web.Models.Summon
{
    public class SummonDetailsVM
    {
        public eCase.Domain.Entities.Summon Summon { get; set; }

        public bool HasPermissions { get; set; }

        public bool HasSummonFile { get; set; }
    }
}