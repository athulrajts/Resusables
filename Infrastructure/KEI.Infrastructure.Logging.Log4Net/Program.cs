using KEI.Infrastructure.Logging;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            _ = SimpleLogConfigurator.Configure()
                .WriteToFile(@"Logs\Testlog.txt")
                .WriteToConsole()
                .WriteToXml(@"Logs\TestLogXml.txt")
                .Finish();

            Logger.Debug("Debug");
            Logger.Info("Info");
            Logger.Warn("Warning !");
            Logger.Error("Error");
            Logger.Fatal("Fatal");

            Console.ReadKey();
        }
    }
}
