using server.Enums;
using server.Services;
using server.Utilities;
using System;

namespace server.Channel
{
    public class LightChannel: IChannel
    {
        private readonly ICLIService _service;
        private int _currentValue;
        private readonly int _maxValue = 255;
        public LightPin Pin { get; set; }

        public int CurrentValue
        {
            get
            {
                return _currentValue;
            }
        }

        public LightChannel()
        {
            _currentValue = 0;
        }

        public LightChannel(ICLIService service)
        {
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

        /// <summary>
        /// Given a duration, return the interval (ms) to turn the lights off
        /// </summary>
        /// <param name="duration_m">How long to take before reaching zero (minutes)</param>
        /// <returns></returns>
        public int GetIntervalToSleep(int duration_m)
        {
            var duration_S = duration_m.MinutesToS();
            var seconds = (double)duration_S / (double)_currentValue;
            var ms = seconds * 1000;

            return (int)Math.Round(ms, 0);
        }

        public bool SetChannelToMaxValue()
        {
            return SetChannelValue(_maxValue);
        }

        public bool SetChannelValuePct(int pctValue)
        {
            var value = (_maxValue * pctValue) / 100;
            return SetChannelValue(value);
        }

        public bool SetChannelValue(int value)
        {
            if (value > _maxValue || value < 0)
            {
                return false;
            }
            
            var command = Commands.SET_CHANNEL_VALUE
                .Replace(Constants.FILLPOINT_CHANNEL_NUMBER, ((int)Pin).ToString())
                .Replace(Constants.FILLPOINT_VALUE, value.ToString());

            var result = _service.ExecuteCommand(command);
            
            if (string.IsNullOrEmpty(result))
            {
                // if result is OK, then we can proceed;
                _currentValue = value;
                return true;
            }

            return false;
        }

        public bool TurnOff()
        {
            if (_currentValue <= 0)
            {
                return false;
            }

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