using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using server.Enums;
using server.Models;
using server.Services;

namespace server
{
    /// <summary>
    /// Controller to handle the interaction with the lights
    ///     - 
    /// </summary>
    public class LightController: ILightController
    {
        private Timer _sleepTimer;
        
        private readonly IEnumerable<Channel> _channels;
        public LightController(ICLIService service)
        {
            _channels = new List<Channel>()
            {
                new Channel(LightPin.CoolWhite, service),
                new Channel(LightPin.WarmWhite, service)
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
            int interval = maxValue.CurrentValue;

            CleanUpSleepTimersAndHandlers();
            _sleepFinishedEvent = sleepFinishedEventHandler;

            var sleepDelayTimer = new Timer(MinutesToMS(delayBeforeStarting_m));
            sleepDelayTimer.Elapsed += (sender, args) =>
            {
                OnSleepStart(interval);
                sleepDelayTimer.Dispose();
            };

            sleepDelayTimer.AutoReset = false;
            sleepDelayTimer.Start();
        }

        public int MinutesToMS(int minutes)
        {
            return minutes * 60 * 1000; 
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