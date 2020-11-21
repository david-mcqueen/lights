using System;
using System.Collections.Generic;
using System.Text;

namespace server.Utilities
{
    public static class Extensions
    {
        public static int MinutesToMS(this int minutes)
        {
            return minutes * 60 * 1000;
        }

        public static int MinutesToS(this int minutes)
        {
            return minutes * 60;
        }
    }
}
