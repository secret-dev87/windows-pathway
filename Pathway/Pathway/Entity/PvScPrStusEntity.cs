using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvScPrStusEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string PathwayName { get; set; }
        public virtual string ScName { get; set; }
        public virtual string ScProcessName { get; set; }
        public virtual int ErrorNumber { get; set; }
        public virtual int ErrorInfo { get; set; }
        public virtual int Pid { get; set; }
        public virtual int ProcLinks { get; set; }
        public virtual int ProcWeight { get; set; }
        public virtual double ScStatusTime { get; set; }
        public virtual double ScProcessTime { get; set; }
        public virtual double DeltaTime { get; set; }
        public virtual double DeltaProcTime { get; set; }
        public virtual string ScprCpu { get; set; }
        public virtual double CurPages { get; set; }
        public virtual double RecQueue { get; set; }
        public virtual double PageFaults { get; set; }
        public virtual string ProcState { get; set; }
    }
}
