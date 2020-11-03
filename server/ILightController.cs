using server.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace server
{
    interface ILightController
    {
        bool SetLightValue(LightPin pin, int pctValue);

        bool TurnOff();
        int MinutesToMS(int minutes);

        void Sleep(EventHandler sleepFinishedEventHandler, int delayBeforeStarting_m);

        void WakeUp(EventHandler wakeupFinishedEventHandler);
    }
}
