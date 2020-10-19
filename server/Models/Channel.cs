using server.Enums;
using server.Services;

namespace server.Models
{
    public class Channel
    {
        private readonly ICLIService _service;
        private int _currentValue;
        private readonly int _maxValue = 255;

        private LightPin _lightPin;

        public LightPin Pin => _lightPin;

        public Channel()
        {
            _currentValue = 0;
        }

        // TODO:- DI the service
        public Channel(LightPin pin, ICLIService service)
        {
            _lightPin = pin;
            _service = service;
            _currentValue = 0;
        }

        public bool DecrementBrightness()
        {
            return SetChannelValue(_currentValue - 1);
        }

        public bool IncrementBrightness()
        {
            return SetChannelValue(_currentValue + 1);
        }

        public bool SetChannelValuePct(int pctValue)
        {
            var value = (_maxValue * pctValue) / 100;
            return SetChannelValue(value);
        }

        public virtual bool SetChannelValue(int value)
        {
            var command = Commands.SET_CHANNEL_VALUE
                .Replace(Constants.FILLPOINT_CHANNEL_NUMBER, ((int)Pin).ToString())
                .Replace(Constants.FILLPOINT_VALUE, value.ToString());

            var result = _service.ExecuteCommand(command);
            
            if (string.IsNullOrEmpty(result))
            {
                // if result is OK, then we can proceed;
                _currentValue = 0;
                return true;
            }

            return false;
        }

        public bool TurnOff()
        {
            var command = Commands.TURN_CHANNEL_OFF.Replace(Constants.FILLPOINT_CHANNEL_NUMBER, ((int)Pin).ToString());

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