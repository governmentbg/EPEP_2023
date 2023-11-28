using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Case
{
    public class HearingParticipantVM
    {
        public string FullName { get; set; }
        public string SubstitudeFor { get; set; }
        public string Role { get; set; }

        public int Order
        {
            get
            {
                if (string.IsNullOrEmpty(Role))
                {
                    return 99;
                }

                if (Role.Contains("пред", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                }
                if (Role.Contains("докл", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 1;
                }
                return 2;
            }
        }
    }
}
