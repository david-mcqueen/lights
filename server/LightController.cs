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

            this._scheduleManager.SleepEpoch += SleepEpoch;
            this._scheduleManager.WakeEpoch += WakeEpoch;
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
        /// Given a value, set that on the light
        /// </summary>
        public bool SetLightValuePct(LightPin pin, int pctValue)
        {
            var channel = _channels.First(c => c.Pin == pin);

            if (channel == null)
            {
                return false;
            }

            return channel.SetChannelValuePct(pctValue);
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

            _scheduleManager.StartSleep(interval);
        }

        /// <summary>
        /// Waits for the delay period, and then starts the Sleep process
        /// </summary>
        /// <param name="sleepDurationMinutes"></param>
        /// <param name="delayMinutes"></param>
        public void SleepWithDelay(int sleepDurationMinutes, int delayMinutes)
        {
            _scheduleManager.StartWithDelay(delayMinutes, sleepDurationMinutes);
        }

        private void SleepEpoch(object sender, EventArgs args)
        {
            _channels.Where(c => c.Pin == LightPin.CoolWhite).First().TurnOff();

            if (!_channels.Where(c => c.Pin == LightPin.WarmWhite).First().DecrementBrightness())
            {
                _scheduleManager.StopTimers();
            }
        }

        public void WakeUp(int wakeDuration)
        {
            _scheduleManager.StartWake(wakeDuration);
        }

        private void WakeEpoch(object sender, EventArgs args)
        {
            var channelsChanged = 0;

            _channels.ToList().ForEach(c =>
            {
                if (c.IncrementBrightness())
                    channelsChanged++;
            });

            // Only stop the process if non of the channels changed
            if (channelsChanged == 0)
            {
                _scheduleManager.StopTimers();
            }
        }
    }
}