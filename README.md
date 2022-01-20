# dotnet-libs
Various bespoke libraries that provide functionality that is useful across different types of solutions.

## Common

### Diagnostics

#### Profiler
The profiler provides a way to determine how long code takes to execute without having to duplicate Stopwatch code in multiple places and with a mechanism to conditionally disable the stopwatch and completion action(s).

A `ProfilerManager` can be registed as a dependency in Startup.cs using the following method:

    services.AddProfiler();

If you want to override the default options you can do so:

    services.AddProfiler(options => {
        options.IsEnabled = hostEnv.IsDevelopment();
    });

You can also customize the completion action:

    services.AddProfiler(options => {
        options.Completion = (name, duration, manager) => manager.Logger.LogTrace($"{name} took {duration.TotalMilliseconds}ms");
    });

When you resolve a dependency for `ProfilerManager`, you can use the `Instantiate()` method to profile some code execution within a `using` block (to call the `Dispose()` method of the `ProfileInstance`):

    public class SomeClass
    {
        private ProfilerManager Manager { get; }

        public SomeClass(ProfilerManager manager)
        {
            Manager = manager;
        }

        public void SomeMethod()
        {
            using(var profiler = Manager.Instantiate("Profiler for Thread.Sleep(1000)"))
            {
                Thread.Sleep(1000);
            }
        }
    }
