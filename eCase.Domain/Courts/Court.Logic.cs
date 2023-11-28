using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class Court
    {
        public string Abbreviation
        {
            get
            {
                return this.Name
                    .Replace("Районен съд", "РС")
                    .Replace("Окръжен съд", "ОС")
                    .Replace("Апелативен съд", "АС")
                    .Replace("Административен съд", "АДМС");
            }
        }
    }
}
