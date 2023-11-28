using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;
using System.Text;

namespace eCase.Domain.Entities
{
    public partial class Lawyer : IAggregateRoot
    {
        public string LawyerDataTable
        {
            get
            {
                StringBuilder info = new StringBuilder();

                info.Append("<table style=\"text-align:left;\">");

                if (!string.IsNullOrEmpty(this.Name))
                    info.AppendFormat("<tr><td>Име:</td> <td>{0}</td></tr>", this.Name);

                if (!string.IsNullOrEmpty(this.Number))
                    info.AppendFormat("<tr><td>Личен №:</td> <td>{0}</td></tr>", this.Number);

                if (!string.IsNullOrEmpty(this.College))
                    info.AppendFormat("<tr><td>Колегия:</td> <td>{0}</td></tr>", this.College);

                info.Append("</table>");

                return info.ToString();
            }
        }
    }
}
