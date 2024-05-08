using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Entity;
using Pathway.Core.Helper;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Repositories
{
    internal class PvErrorsRepository : IPvErrorsRepository
    {
        public PvErrorsRepository() { }

        public ErrorInfo GetErrorInfo(long errorNumber)
        {
            var errorInfo = new ErrorInfo();

            using(var session = NHibernateHelper.OpenSystemSession())
            {
                var error = session.Query<PvErrorEntity>()
                    .Where(x => x.ErrorNumber == errorNumber)
                    .First();

                if (error != null)
                {
                    errorInfo.ErrorNumber = errorNumber;
                    errorInfo.Message = error.Message;
                    errorInfo.Cause = error.Cause;
                    errorInfo.Effect = error.Effect;
                    errorInfo.Recovery = error.Recovery;
                }
            }

            return errorInfo;
        }
    }
}
