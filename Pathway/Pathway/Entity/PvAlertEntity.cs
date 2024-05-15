using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvAlertEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual DateTime ToTimestamp { get; set; }
        public virtual string Pathway { get; set; }
        public virtual int TermHiMaxRT { get; set; }
        public virtual int TermHiAvgRT { get; set; }
        public virtual int TermUnused { get; set; }
        public virtual int TermErrorList { get; set; }
        public virtual int TCPQueuedTransactions { get; set; }
        public virtual int TCPLowTermPool { get; set; }
        public virtual int TCPLowServerPool { get; set; }
        public virtual int TCPUnused { get; set; }
        public virtual int TCPErrorList { get; set; }
        public virtual int ServerHiMaxRT { get; set; }
        public virtual int ServerHiAvgRT { get; set; }
        public virtual int ServerQueueTCP { get; set; }
        public virtual int ServerQueueLinkmon { get; set; }
        public virtual int ServerUnusedClass { get; set; }
        public virtual int ServerUnusedProcess { get; set; }
        public virtual int ServerErrorList { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvAlertEntity other = (PvAlertEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ToTimestamp == other.ToTimestamp && Pathway == other.Pathway)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" + 
                    ToTimestamp.ToString() + "|" + 
                    Pathway.ToString()
                ).GetHashCode();
        }
    }
}
