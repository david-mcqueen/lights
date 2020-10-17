using System.Collections.Generic;
using System.Linq;
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
        public void SetLightValue(int pctValue)
        { 
            
        }

        /// <summary>
        /// Turn off all of the light Channels
        /// </summary>
        public bool TurnOff()
        {
            var success =  _channels.All(c => c.TurnOff());

            return success;
        }
    }
}