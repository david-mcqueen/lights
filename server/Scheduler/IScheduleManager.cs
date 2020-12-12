using System;
using System.Collections.Generic;
using System.Text;

namespace server.Scheduler
{
    public interface IScheduleManager
    {
        public event EventHandler<EventArgs> Epoch;
        void Start(int interval);
        void Stop();
    }
}
