namespace server
{
    public static class Commands
    {
        public static readonly string TURN_CHANNEL_OFF = $"pigs p {Constants.FILLPOINT_CHANNEL_NUMBER} 0";

        public static readonly string SET_CHANNEL_VALUE = $"pigs p {Constants.FILLPOINT_CHANNEL_NUMBER} {Constants.FILLPOINT_VALUE}";


    }
}