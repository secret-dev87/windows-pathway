using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Entity
{
    public class PathwayAllAlertsEntity
    {
        virtual public int AlertId { get; set; }
        virtual public int AlertType { get; set; }
        virtual public string AlertName { get; set; }
    }
}
