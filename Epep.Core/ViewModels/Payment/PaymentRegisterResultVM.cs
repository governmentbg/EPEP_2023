using Epep.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Payment
{
    public class PaymentRegisterResultVM : SaveResultVM
    {
        public string CardFormUrl { get; set; }
    }
}
