using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScStusEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual int ErrorNumber { get; set; }
        public virtual int ErrorInfo { get; set; }
        public virtual int ScRunning { get; set; }
        public virtual string FreezeState { get; set; }
    }
}
