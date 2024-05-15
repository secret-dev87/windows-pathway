using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PvErrorEntity
    {
        public virtual DateTime FromTimestamp { get; set; }
        public virtual int ErrorNumber { get; set; }
        public virtual string Message { get; set; }
        public virtual string Cause { get; set; }
        public virtual string Effect { get; set; }
        public virtual string Recovery { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvErrorEntity other = (PvErrorEntity)obj;

            if (other == null)
                return false;
            if (FromTimestamp == other.FromTimestamp && ErrorNumber == other.ErrorNumber)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                    FromTimestamp.ToString() + "|" +
                    ErrorNumber.ToString()
                ).GetHashCode();
        }
    }
}
