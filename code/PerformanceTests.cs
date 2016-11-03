using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class PerformanceTests
    {
        private class StopwatchRegion : IDisposable
        {
            private Stopwatch sw;

            public StopwatchRegion()
            {
                sw = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    sw.Stop();
                    Console.WriteLine(sw.ElapsedMilliseconds + " ms.");
                }
            }
        }
        [TestMethod]
        public void TestSimplePerformance()
        {
            using (new StopwatchRegion())
            {
                var values = new List<int>();
                for (int i = 0; i < 10000; i ++)
                {
                    values.Add(ComplicatedCalculation(i));
                }
            }
        }

        [TestMethod]
        public void TestUnlockedParallel()
        {
            using (new StopwatchRegion())
            {
                var values = new List<int>();
                Parallel.For(0,
                    500,
                    i =>
                    {
                        values.Add(ComplicatedCalculation(i));
                    });
            }
        }

        [TestMethod]
        public void TestLockedParallelLock()
        {
            object sync = new object();
            using (new StopwatchRegion())
            {
                var values = new List<int>();
                Parallel.For(0,
                    10000,
                    i =>
                    {
                        lock (sync)
                        {
                            values.Add(ComplicatedCalculation(i));
                        }
                    });
            }
        }

        [TestMethod]
        public void TestUnlockedParallelLockConcurrentBag()
        {
            using (new StopwatchRegion())
            {
                var values = new ConcurrentBag<int>();
                Parallel.For(0,
                    10000,
                    i =>
                    {
                        values.Add(ComplicatedCalculation(i));
                    });
            }
        }

        [TestMethod]
        public void TestUnlockedParallelArray()
        {
            using (new StopwatchRegion())
            {
                var values = new int[10000];
                Parallel.For(0,
                    10000,
                    i =>
                    {
                        values[i] = ComplicatedCalculation(i);
                    });
            }
        }

        private int ComplicatedCalculation(int i)
        {
            Task.Delay(1).Wait();
            return i;
        }
    }
}