using System.Collections.Generic;

namespace eCase.Common.Enums
{
    public class SummonTypeNomenclature
    {
        public string Text { get; set; }

        public string Code { get; set; }

        public static readonly SummonTypeNomenclature Act = new SummonTypeNomenclature { Text = "Съдебен акт", Code = "1" };
        public static readonly SummonTypeNomenclature Appeal = new SummonTypeNomenclature { Text = "Обжалване на съдебен акт", Code = "2" };
        public static readonly SummonTypeNomenclature Case = new SummonTypeNomenclature { Text = "Съдебно дело", Code = "3" };
        public static readonly SummonTypeNomenclature Hearing = new SummonTypeNomenclature { Text = "Съдебно заседание", Code = "4" };

        public static readonly IEnumerable<SummonTypeNomenclature> Values =
            new List<SummonTypeNomenclature>
            {
                Act,
                Appeal,
                Case,
                Hearing
            };
    }
}