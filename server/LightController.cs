using server.Channel;
using server.Enums;
using server.Scheduler;
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
        private Timer _sleepTimer;
        
        private readonly IEnumerable<IChannel> _channels;
        private readonly IScheduleManager _scheduleManager;

        public LightController(ICLIService service, IScheduleManager scheduleManager)
        {
            _channels = new List<IChannel>()
            {
                ChannelFactory.GetChannelForPin(service, LightPin.CoolWhite),
                ChannelFactory.GetChannelForPin(service, LightPin.WarmWhite)
            };
            this._scheduleManager = scheduleManager;

            this._scheduleManager.Epoch += SleepEpoch;
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

        /// <summary>
        /// Starts Sleep, specifying how much to delay by, and completion event handler
        /// </summary>
        /// <param name="sleepDurationMinutes">How long the sleep process should take</param>
        public void Sleep(int sleepDurationMinutes)
        {
            var coolChannel = _channels.Where(c => c.Pin == LightPin.CoolWhite).First();
            var warmChannel = _channels.Where(c => c.Pin == LightPin.WarmWhite).First();

            coolChannel.TurnOff();

            int interval = warmChannel.GetIntervalToSleep(sleepDurationMinutes);

            _scheduleManager.Start(interval);
        }

        private void SleepEpoch(object sender, EventArgs args)
        {
            if (!_channels.Where(c => c.Pin == LightPin.WarmWhite).First().DecrementBrightness())
            {
                _scheduleManager.Stop();
            }
        }

        public void WakeUp(EventHandler wakeupFinishedEventHandler)
        {
            throw new NotImplementedException();
        }
    }
}