namespace server.Scheduler
{
    public class SleepWithDelayTimer : System.Timers.Timer
    {
        public SleepWithDelayTimer(double interval)
            : base(interval)
        {
        }

        public int SleepInterval { get; set; }
    }
}
