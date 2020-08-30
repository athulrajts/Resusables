using System;
using System.Diagnostics;

namespace KEI.Infrastructure.Timing
{
    public static class Benchmarking
    {
        public static TimeSpan Measure(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            
            action();
            
            stopwatch.Stop();
            
            return stopwatch.Elapsed;
        }

        public static TimeSpan Benchmark(Action action, int iterations = 50, int warmup = 3)
        {
            var stopwatch = new Stopwatch();
            
            for (int i = 0; i < iterations + warmup; i++)
            {
                if (i == warmup - 1)
                {
                    stopwatch.Start();
                }
            }

            stopwatch.Stop();

            return TimeSpan.FromTicks(stopwatch.Elapsed.Ticks / iterations);
        }
    }
}
