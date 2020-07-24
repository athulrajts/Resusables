using System;

namespace KEI.Infrastructure.PeriodicTasks
{
    interface IPeriodicTask
    {
        string Name { get; set; }
        Period Period { get; set; }
        DateTime LastExecutedOn { get; set; }
        bool CanExecute();
        void Pause();
        void Resume();
    }

    public enum Period
    {
        OneMinute = 1,
        FiveMinutes = 5,
        TenMinutes = 10, 
        ThirtyMinutes = 30,
        OneHour =  60,
        SixHours = 3600,
        OneDay = 1440,
        OneWeek = 10080,
        OneMonth = 302400
    }
}
