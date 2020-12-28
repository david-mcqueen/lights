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

        public void StartWithDelay(int delay, int interval)
        {
            // After the delay, call `Start(interval);`
            throw new NotImplementedException();
        }
    }
}
