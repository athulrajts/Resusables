using System;

namespace KEI.Infrastructure
{
    public class SplashScreenLogger
    {
        public event EventHandler<string> OnNewMessage;

        public void Log(string message)
        {
            OnNewMessage?.Invoke(this, message);
        }

        private static SplashScreenLogger instance;
        public static SplashScreenLogger Instance => instance ?? (instance = new SplashScreenLogger());
    }


}
