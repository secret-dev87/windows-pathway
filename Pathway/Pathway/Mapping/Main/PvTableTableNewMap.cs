using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity.Main;

namespace Pathway.Core.Mapping.Main
{
    public class PvTableTableNewMap : ClassMap<PvTableTableNewEntity>
    {
        public PvTableTableNewMap()
        {
            Table("_pvtabletablenew");
            CompositeId()
                .KeyProperty(x => x.TableName, "TableName")
                .KeyProperty(x => x.FSeq, "FSeq")
                .KeyProperty(x => x.FName, "FName");
            Map(x => x.FPlus);
            Map(x => x.FType).Length(32);
            Map(x => x.FSize);
            Map(x => x.PKSeq);
            Map(x => x.XSeq);
            Map(x => x.SourceName).Length(64);
            Map(x => x.SSeq);
            Map(x => x.SOff);
            Map(x => x.SGString).Length(64);
            Map(x => x.SFName).Length(64);
            Map(x => x.SFTypeID).Length(20);
            Map(x => x.SFType).Length(32);
            Map(x => x.SFormat).Length(64);
            Map(x => x.SComment).Length(255);
            Map(x => x.Format).Length(64);
            Map(x => x.Description).Length(255);
        }
    }
}
