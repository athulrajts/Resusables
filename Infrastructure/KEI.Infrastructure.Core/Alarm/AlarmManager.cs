using KEI.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEI.Infrastructure.Alarm
{
    public class AlarmManager : ConfigHolder<List<AlarmInfo>>, IAlarmManager
    {
        public AlarmManager(IViewService viewService) : base(viewService)
        {

        }

        public bool HasActiveAlarms => ActiveAlarms.Count > 0;

        public ObservableCollection<Alarm> ActiveAlarms { get; set; } = new ObservableCollection<Alarm>();

        public ObservableCollection<Alarm> AlarmHistory { get; set; } = new ObservableCollection<Alarm>();

        public override string ConfigPath => @"Configs/alarms.config";

        public override string ConfigName => @"Alarms";

        public void ClearAlarm(string alarmID)
        {
            if (ActiveAlarms.SingleOrDefault(x => x.AlarmDisplayTitle == alarmID) is Alarm alarm)
            {
                ActiveAlarms.Remove(alarm);

                alarm.ResolutionTime = DateTime.Now;

                AlarmHistory.Add(alarm);
            }
        }

        public void RaiseAlarm(string alarmID)
        {
            if (Config.Find(alarmInfo => alarmID == $"ERR_{alarmInfo.AlarmOwner.Abbreviation}_{alarmInfo.AlarmID}") is AlarmInfo info)
            {
                if (ActiveAlarms.SingleOrDefault(x => x.AlarmDisplayTitle == alarmID) is null)
                {
                    // Show Dialog
                    ActiveAlarms.Add(new Alarm(info));
                }
            }
        }
    }
}
