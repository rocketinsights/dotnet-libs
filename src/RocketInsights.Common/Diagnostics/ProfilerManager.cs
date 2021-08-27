using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RocketInsights.Common.Diagnostics
{
    public class ProfilerManager
    {
        public ProfilerManagerOptions Options { get; private set; }
        public ILogger Logger { get; private set; }

        public ProfilerManager(IOptions<ProfilerManagerOptions> options, ILogger<ProfilerManager> logger)
        {
            Options = options.Value;
            Logger = logger;
        }

        public ProfilerInstance Instantiate(string name)
        {
            return new ProfilerInstance(name, this);
        }
    }
}
