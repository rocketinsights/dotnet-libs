using System;
using System.Diagnostics;

namespace RocketInsights.Common.Diagnostics
{
    public class ProfilerInstance : IDisposable
    {
        public string Name { get; private set; }
        private ProfilerManager Manager { get; }
        private Stopwatch Stopwatch { get; }

        public ProfilerInstance(string name, ProfilerManager manager)
        {
            Name = name;
            Manager = manager;

            Stopwatch = new Stopwatch();

            if (manager.Options.IsEnabled)
            {
                Stopwatch.Start();
            }
        }

        public void Dispose()
        {
            if(Manager.Options.IsEnabled)
            {
                Stopwatch.Stop();
                Manager.Options.Completion.Invoke(Name, Stopwatch.Elapsed, Manager);
            }
        }
    }
}
