using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Case : IAggregateRoot
    {
        public string Abbreviation
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if(this.CaseKind != null)
                {
                    sb.Append(this.CaseKind.Abbreviation);
                    sb.Append(" ");
                }
                sb.Append(this.Number);
                sb.Append("/");
                sb.Append(this.CaseYear);

                return sb.ToString();
            }
        }
    }
}
