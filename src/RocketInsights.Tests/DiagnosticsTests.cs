using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common.Diagnostics;
using System.Threading;
using System;

namespace RocketInsights.Tests
{
    [TestClass]
    public class DiagnosticsTests
    {
        private string Name { get; set; }
        private TimeSpan Duration { get; set; }

        private static IServiceCollection ServiceCollection => new ServiceCollection().AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace).AddDebug());

        public DiagnosticsTests()
        {

        }

        [TestMethod]
        public void ProfilerDefaultOptions()
        {
            // Assemble
            var serviceProvider = ServiceCollection
                .AddProfiler(options =>
                {
                    options.Completion = (name, duration, manager) =>
                    {
                        Name = name;
                        Duration = duration;
                    };
                })
                .BuildServiceProvider();

            var manager = serviceProvider.GetRequiredService<ProfilerManager>();

            // Act
            using (var profiler = manager.Instantiate("Profiler (Default Options)"))
            {
                Thread.Sleep(1000);
            }

            // Assert
            Assert.AreEqual("Profiler (Default Options)", Name);
            Assert.IsTrue(Duration >= TimeSpan.FromMilliseconds(1000));
        }

        [TestMethod]
        public void ProfilerIsEnabledFalse()
        {
            // Assemble
            Name = string.Empty;
            Duration = TimeSpan.Zero;

            var serviceProvider = ServiceCollection
                .AddProfiler(options =>
                {
                    options.Completion = (name, duration, manager) =>
                    {
                        Name = name;
                        Duration = duration;
                    };

                    options.IsEnabled = false;
                })
                .BuildServiceProvider();

            var manager = serviceProvider.GetRequiredService<ProfilerManager>();

            // Act
            using (var profiler = manager.Instantiate("Profiler (IsEnabled = false)"))
            {
                Thread.Sleep(1000);
            }

            // Assert
            Assert.AreEqual(string.Empty, Name);
            Assert.AreEqual(TimeSpan.Zero, Duration);
        }
    }
}