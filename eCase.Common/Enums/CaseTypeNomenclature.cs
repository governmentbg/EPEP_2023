using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eCase.Common.Enums
{
    public class CaseTypeNomenclature
    {
        public string Text { get; set; }
        public string Code { get; set; }

        public static readonly CaseTypeNomenclature Criminal = new CaseTypeNomenclature { Text = "Наказателно", Code = "0001" };
        public static readonly CaseTypeNomenclature Civil = new CaseTypeNomenclature { Text = "Гражданско", Code = "0002" };
        public static readonly CaseTypeNomenclature Administrative = new CaseTypeNomenclature { Text = "Административно", Code = "0003" };

        public static readonly IEnumerable<CaseTypeNomenclature> Values =
            new List<CaseTypeNomenclature>
            {
                Criminal,
                Civil,
                Administrative,
            };
    }
}
