namespace server.Models
{
    public interface IChannel
    {
        bool DecrementBrightness();

        bool IncrementBrightness();

        bool GetChannelValuePct(int pctValue);

        bool SetChannelValue(int value);
    }
}