using System;
using System.Collections.Generic;
using System.Text;

namespace server.Scheduler
{
    public interface IScheduleManager
    {
        public event EventHandler<EventArgs> SleepEpoch;
        public event EventHandler<EventArgs> WakeEpoch;
        void StartSleep(int interval);
        void StartWithDelay(int delay, int interval);
        void StartWake(int interval);
        void StopTimers();
    }
}
