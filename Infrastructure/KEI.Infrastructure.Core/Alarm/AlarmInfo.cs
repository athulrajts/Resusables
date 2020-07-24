using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Alarm
{
    public class AlarmInfo
    {
        public int AlarmID { get; set; }
        public AlarmSeverity Serverity { get; set; }
        public string AlarmText { get; set; }
        public List<string> Remedies { get; set; }
        public AlarmAction AlarmAction { get; set; }
        public AlarmOwner AlarmOwner { get; set; }
    }

    public enum AlarmSeverity
    {
        High,
        Moderate,
        Low
    }

    public class AlarmOwner
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }

    public enum AlarmAction
    {
        None,
        Stop,
        Pause
    }
}
