using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class CurrentTableEntity
    {
        public virtual string TableName { get; set; }
        public virtual int EntityId { get; set; }
        public virtual string SystemSerial { get; set; }
        public virtual int Interval { get; set; }
        public virtual DateTime DataDate { get; set; }
        public virtual string MeasureVersion { get; set; }
    }
}
