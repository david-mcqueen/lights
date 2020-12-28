using System;
using System.Collections.Generic;
using System.Text;

namespace server.Scheduler
{
    public class ScheduleManager : IScheduleManager
    {
        private SleepWithDelayTimer _startWithDelayTimer;
        private System.Timers.Timer _sleepTimer;

        public event EventHandler<EventArgs> Epoch;


        public void Start(int interval)
        {
            _sleepTimer = new System.Timers.Timer(interval);
            _sleepTimer.Elapsed += new System.Timers.ElapsedEventHandler(Epoch);
            _sleepTimer.AutoReset = true;
            _sleepTimer.Start();
        }

        public void Stop()
        {
            if (_sleepTimer != null)
            {
                _sleepTimer.Stop();
                _sleepTimer.Dispose();
            }
        }

        public void StartWithDelay(int delay, int interval)
        {
            _startWithDelayTimer = new SleepWithDelayTimer(delay);
            _startWithDelayTimer.SleepInterval = interval;
            _startWithDelayTimer.Elapsed += startWithDelayEvent;
            _startWithDelayTimer.AutoReset = false;
            _startWithDelayTimer.Start();
        }

        private void startWithDelayEvent(object sender, EventArgs args)
        {
            SleepWithDelayTimer timer = (SleepWithDelayTimer)sender;
            Start(timer.SleepInterval);

            _startWithDelayTimer.Stop();
            _startWithDelayTimer.Dispose();
        }
    }
}
