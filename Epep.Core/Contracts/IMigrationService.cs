using Epep.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.Contracts
{
    public interface IMigrationService
    {
        Task<SaveResultVM> MigrateData();
    }
}
