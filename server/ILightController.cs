using server.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace server
{
    public interface ILightController
    {
        bool SetLightValue(LightPin pin, int pctValue);

        bool TurnOff();

        void Sleep(int sleepDurationMinutes);

        void SleepWithDelay(int sleepDurationMinutes, int delayMinutes);

        void WakeUp(EventHandler wakeupFinishedEventHandler);
    }
}
