using Core.Interfaces;
using Core.Model.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class DateService : IDateService
    {
        public DateService()
        {
        }

        public long generateUnixTimeStampMilliseconds()
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentDate = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond) * 1000;

            return currentDate;
        }
    }
}