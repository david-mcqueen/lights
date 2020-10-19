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
        private Timer sleepTimer;
        
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
        
        public async Task Sleep(int interval = 0)
        {
            // var interval = 2000;
            
            sleepTimer = new Timer(interval);
            sleepTimer.Elapsed += (sender, args) =>
            {
                // Every epoch, decrement brightness
                if (!_channels.All(c => c.DecrementBrightness()))
                {
                    sleepTimer.Stop();
                }
            };
            
            sleepTimer.AutoReset = true;
            sleepTimer.Start();
        }
    }
}