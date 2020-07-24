namespace KEI.Infrastructure
{
    public static class ViewService
    {
        public static IViewService Service { get; set; }

        public static void SetBusy(string[] msg = null) => Service?.SetBusy(msg);

        public static void SetBusy(string msg) => Service?.SetBusy(msg);

        public static void SetAvailable() => Service?.SetAvailable();

        public static void UpdateBusyText(string[] msg = null) => Service?.UpdateBusyText(msg);
        
        public static void UpdateBusyText(string msg) => Service?.UpdateBusyText(msg);

        public static void Error(string alert) => Service?.Error(alert);

        public static void Warn(string warning) => Service?.Warn(warning);

        public static void Inform(string info) => Service?.Inform(info);

    }
}
