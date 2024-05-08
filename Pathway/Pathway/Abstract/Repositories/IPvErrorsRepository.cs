using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvErrorsRepository
    {
        ErrorInfo GetErrorInfo(long errorNumber);
    }
}
