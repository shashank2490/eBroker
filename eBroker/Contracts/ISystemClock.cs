﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Contracts
{
    public interface ISystemClock
    {
        DateTime GetCurrentTime();
    }
}
