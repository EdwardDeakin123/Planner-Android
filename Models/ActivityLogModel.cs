using System;
using Front_End.Backend;

namespace Front_End.Models
{
    public class ActivityLogModel : IBackendType
    {
        public int ActivityLogId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public virtual UserModel User { get; set; }
        public virtual ActivityModel Activity { get; set; }
    }
}