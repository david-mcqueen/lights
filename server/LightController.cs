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
    public class LightController
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
        /// Starts sleeping the lights.
        /// Calls the event after it has finished
        /// </summary>
        /// <param name="sleepFinishedEventHandler"></param>
        /// <param name="interval">How long between each decrement to wait before decrementing again</param>
        public void Sleep(EventHandler sleepFinishedEventHandler, int delayBeforeStarting, int interval)
        {
            // TODO:- Should we get the interval outselves? SHould it be total duration?
            CleanUpSleepTimersAndHandlers();
            _sleepFinishedEvent = sleepFinishedEventHandler;

            var sleepDelayTimer = new Timer(delayBeforeStarting);
            sleepDelayTimer.Elapsed += (sender, args) =>
            {
                OnSleepStart(interval);
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
    }
}