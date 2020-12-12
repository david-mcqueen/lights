using server.Enums;
using server.Services;

namespace server.Channel
{
    class ChannelFactory
    {
        public static IChannel GetChannelForPin(ICLIService service, LightPin pin)
        {
            return new LightChannel(service)
            {
                Pin = pin
            };
        }
    }
}
