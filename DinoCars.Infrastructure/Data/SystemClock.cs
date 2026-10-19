using DinoCars.Applogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic
{
    internal sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
