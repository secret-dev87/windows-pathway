using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Pathway.Core.Entity;

namespace Pathway.Core.Mapping
{
    public class PvTermInfoMap : ClassMap<PvTermInfoEntity>
    {
        public PvTermInfoMap()
        {
            Table("pvterminfo");
            CompositeId()
                .KeyProperty(x => x.FromTimestamp, "FromTimestamp")
                .KeyProperty(x => x.ToTimestamp, "ToTimestamp")
                .KeyProperty(x => x.PathwayName, "PathwayName")
                .KeyProperty(x => x.TcpName, "TcpName")
                .KeyProperty(x => x.TermName, "TermName");
            Map(x => x.TFiller).Length(1);
            Map(x => x.AutoRestart);
            Map(x => x.DisplayPages);
            Map(x => x.InitialProgram).Length(30);
            Map(x => x.MaxInputMsgs);
            Map(x => x.TermSubType);
            Map(x => x.TrailingBlanks).Length(1);
            Map(x => x.BreakTerm).Length(1);
            Map(x => x.Diagnostic).Length(1);
            Map(x => x.EchoTerm).Length(1);
            Map(x => x.ExclusiveOnOff).Length(1);
            Map(x => x.FileName).Length(35);
            Map(x => x.InspectOnOff).Length(1);
            Map(x => x.InspectFile).Length(35);
            Map(x => x.IOProtocol).Length(1);
            Map(x => x.PrinterIsAttached).Length(1);
            Map(x => x.PrinterFile).Length(35);
            Map(x => x.TclProg).Length(35);
            Map(x => x.TmfTerm).Length(1);
            Map(x => x.TermType).Length(15);
        }
    }
}
