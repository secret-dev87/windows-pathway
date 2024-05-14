using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvTermStusEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string TermName { get; set; }
        public virtual int ErrorNumber { get; set; }
        public virtual int ErrorInfo { get; set; }
        public virtual string TcpName { get; set; }
        public virtual string FileName { get; set; }
        public virtual string State { get; set; }
    }
}
