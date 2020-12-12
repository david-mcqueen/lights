using server.Enums;

namespace server.Channel
{
    public interface IChannel
    {
        public LightPin Pin { get; }

        bool DecrementBrightness();

        bool IncrementBrightness();

        public int GetIntervalToSleep(int duration_m);

        public bool SetChannelToMaxValue();

        public bool SetChannelValuePct(int pctValue);

        bool SetChannelValue(int value);

        public bool TurnOff();
    }
}