using System;

namespace server.Scheduler
{
    public class ScheduleManager : IScheduleManager
    {
        private SleepWithDelayTimer _startWithDelayTimer;
        private System.Timers.Timer _sleepTimer;
        private System.Timers.Timer _wakeTimer;

        public event EventHandler<EventArgs> SleepEpoch;
        public event EventHandler<EventArgs> WakeEpoch;


        public void StartSleep(int interval)
        {
            _sleepTimer = new System.Timers.Timer(interval);
            _sleepTimer.Elapsed += new System.Timers.ElapsedEventHandler(SleepEpoch);
            _sleepTimer.AutoReset = true;
            _sleepTimer.Start();
        }

        public void StartWake(int interval)
        {
            _wakeTimer = new System.Timers.Timer(interval);
            _wakeTimer.Elapsed += new System.Timers.ElapsedEventHandler(WakeEpoch);
            _wakeTimer.AutoReset = true;
            _wakeTimer.Start();
        }

        public void StopTimers()
        {
            if (_sleepTimer != null)
            {
                _sleepTimer.Stop();
                _sleepTimer.Dispose();
            }

            if (_wakeTimer != null)
            {
                _wakeTimer.Stop();
                _wakeTimer.Dispose();
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
            StartSleep(timer.SleepInterval);

            _startWithDelayTimer.Stop();
            _startWithDelayTimer.Dispose();
        }
    }
}
