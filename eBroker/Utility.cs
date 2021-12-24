using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker
{
    public class Utility
    {
        public static bool IsValidTransactionTime(DateTime time)
        {
            if ((int)time.DayOfWeek < 1 || (int)time.DayOfWeek > 5 || time.TimeOfDay.TotalMinutes < new TimeSpan(9, 0, 0).TotalMinutes || time.TimeOfDay.TotalMinutes > new TimeSpan(15, 0, 0).TotalMinutes)
            {
                return false;
            }

            return true;
        }
    }
}
