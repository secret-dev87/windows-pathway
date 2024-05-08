using System;

namespace Pathway.Core.Infrastructure {
    public class CollectionInfo {
        public DateTime FromTimestamp { get; set; }
        public DateTime ToTimestamp { get; set; }
        public int IntervalNumber { get; set; }
        public string IntervalType { get; set; }
        public bool IsAlert { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsDuplicate { get; set; }
    }
}
