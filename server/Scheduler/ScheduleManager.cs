using System;
using System.Collections.Generic;
using System.Text;

namespace server.Scheduler
{
    public class ScheduleManager : IScheduleManager
    {
        public event EventHandler<EventArgs> Epoch;

        public void Start(int interval)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
