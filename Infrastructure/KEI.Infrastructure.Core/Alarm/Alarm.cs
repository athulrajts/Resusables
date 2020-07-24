using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Alarm
{
    public class Alarm
    {
        public AlarmInfo Info { get; set; }
        public string AlarmDisplayTitle => $"ERR_{Info.AlarmOwner.Abbreviation}_{Info.AlarmID}";
        public DateTime StartTime { get; set; }
        public DateTime ResolutionTime { get; set; }

        public Alarm()
        {
            StartTime = DateTime.Now;
        }

        public Alarm(AlarmInfo info)
        {
            Info = info;
            StartTime = DateTime.Now;
        }
    }
}
