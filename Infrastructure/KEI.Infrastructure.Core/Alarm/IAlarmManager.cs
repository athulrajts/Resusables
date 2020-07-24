using System.Collections.ObjectModel;

namespace KEI.Infrastructure.Alarm
{
    public interface IAlarmManager
    {
        bool HasActiveAlarms { get; }
        ObservableCollection<Alarm> ActiveAlarms { get; set; }
        ObservableCollection<Alarm> AlarmHistory { get; set; }

        void RaiseAlarm(string alarmID);
        void ClearAlarm(string alarmID);
    }
}
