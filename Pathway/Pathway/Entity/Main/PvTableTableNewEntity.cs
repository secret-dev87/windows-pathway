using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity.Main
{
    public class PvTableTableNewEntity
    {
        public virtual string TableName { get; set; }
        public virtual int FSeq { get; set; }
        public virtual int FPlus { get; set; }
        public virtual string FName { get; set; }
        public virtual string FType { get; set; }
        public virtual int FSize { get; set; }
        public virtual int PKSeq { get; set; }
        public virtual double XSeq { get; set; }
        public virtual string SourceName { get; set; }
        public virtual int SSeq { get; set; }
        public virtual int SOff { get; set; }
        public virtual string SGString { get; set; }
        public virtual string SFName { get; set; }
        public virtual string SFTypeID { get; set; }
        public virtual string SFType { get; set; }
        public virtual string SFormat { get; set; }
        public virtual string SComment { get; set; }
        public virtual string Format { get; set; }
        public virtual string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            PvTableTableNewEntity other = (PvTableTableNewEntity)obj;

            if (other == null)
                return false;
            if (TableName == other.TableName &&
                FSeq == other.FSeq &&
                FName == other.FName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (
                TableName.ToString() + "|" +
                FSeq.ToString() + "|" +
                FName.ToString()
            ).GetHashCode();
        }
    }
}
