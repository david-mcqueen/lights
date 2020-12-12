using server.Channel;
using server.Enums;
using server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace server
{
    /// <summary>
    /// Controller to handle the interaction with the lights
    ///     - 
    /// </summary>
    public class LightController: ILightController
    {
        private const int SLEEP_DURATION_MINUTES = 30;
        private Timer _sleepTimer;
        
        private readonly IEnumerable<IChannel> _channels;
        public LightController(ICLIService service)
        {
            _channels = new List<IChannel>()
            {
                ChannelFactory.GetChannelForPin(service, LightPin.CoolWhite),
                ChannelFactory.GetChannelForPin(service, LightPin.WarmWhite)
            };
        }
        /// <summary>
        /// Given a value, set that on the light
        /// </summary>
        public bool SetLightValue(LightPin pin, int pctValue)
        {
            var channel = _channels.First(c => c.Pin == pin);

            if (channel == null)
            {
                return false;
            }
            
            return channel.SetChannelValue(pctValue);
        }

        /// <summary>
        /// Turn off all of the light Channels
        /// </summary>
        public bool TurnOff()
        {
            var success =  _channels.All(c => c.TurnOff());

            return success;
        }

        private event EventHandler _sleepFinishedEvent;

        /// <summary>
        /// Starts Sleep, specifying how much to delay by, and completion event handler
        /// </summary>
        /// <param name="sleepFinishedEventHandler">Handler to be notified on Sleep finishing</param>
        /// <param name="delayBeforeStarting_m">How long to delay by</param>
        public void Sleep(EventHandler sleepFinishedEventHandler, int delayBeforeStarting_m)
        {
            var maxValue = _channels.Where(c => c.Pin == LightPin.WarmWhite).First();
            int interval = maxValue.GetIntervalToSleep(SLEEP_DURATION_MINUTES);

            CleanUpSleepTimersAndHandlers();
            _sleepFinishedEvent = sleepFinishedEventHandler;

            var sleepDelayTimer = new Timer(interval);
            sleepDelayTimer.Elapsed += (sender, args) =>
            {
                OnSleepStart(1);
                sleepDelayTimer.Dispose();
            };

            sleepDelayTimer.AutoReset = false;
            sleepDelayTimer.Start();
        }

        private void OnSleepStart(int interval)
        {
            _sleepTimer = new Timer(interval);
            _sleepTimer.Elapsed += (sender, args) =>
            {
                // Every epoch, decrement brightness. Then stop sleeping
                if (!_channels.All(c => c.DecrementBrightness()))
                {
                    _sleepTimer.Stop();
                    OnSleepFinished(EventArgs.Empty);
                }
            };
            
            _sleepTimer.AutoReset = true;
            _sleepTimer.Start();
        }

        private void OnSleepFinished(EventArgs e)
        {
            _sleepFinishedEvent?.Invoke(this, e);
            CleanUpSleepTimersAndHandlers();
        }

        private void CleanUpSleepTimersAndHandlers()
        {
            _sleepTimer?.Dispose();
            _sleepFinishedEvent = null;
        }

        public void WakeUp(EventHandler wakeupFinishedEventHandler)
        {
            throw new NotImplementedException();
        }
    }
}