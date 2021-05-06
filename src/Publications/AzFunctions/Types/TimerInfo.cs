using PublicationsAzFunctions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicationsAzFunctions.Types
{
    public class TimerInfo
    {
        public ScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }
}
