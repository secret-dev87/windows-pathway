using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract {
    internal interface IPvErrors {
        ErrorInfo GetErrorInfo(long errorNumber);
    }
}