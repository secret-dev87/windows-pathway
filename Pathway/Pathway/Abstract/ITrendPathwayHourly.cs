using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Pathway.Core.Abstract {
	interface ITrendPathwayHourly {
		DataTable GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp);
	}
}
