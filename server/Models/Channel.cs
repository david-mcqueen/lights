using server.Enums;
using server.Services;

namespace server.Models
{
    public class Channel
    {
        private readonly ICLIService _service;
        private readonly int _pin;
        private int _currentValue;


        // TODO:- DI the service
        public Channel(LightPin pin, ICLIService service)
        {
            _pin = (int) pin;
            _service = service;
            _currentValue = 0;
        }

        public bool TurnOff()
        {
            var command = Commands.TURN_CHANNEL_OFF.Replace(Constants.FILLPOINT_CHANNEL_NUMBER, _pin.ToString());

            var result = _service.ExecuteCommand(command);

            if (string.IsNullOrEmpty(result))
            {
                // if result is OK, then we can proceed;
                _currentValue = 0;
                return true;
            }

            return false;
        }
    }
}