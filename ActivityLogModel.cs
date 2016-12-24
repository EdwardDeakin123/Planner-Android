using System;

namespace DragAndDropDemo
{
    class ActivityLogModel : IBackendType
    {
        public int ActivityLogId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public virtual UserModel User { get; set; }
        public virtual ActivityModel Activity { get; set; }
    }
}