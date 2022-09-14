using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public interface IKontentApiEngine
    {
        Task<string?> GetContentItem(string codename, string? elements = null, string? systemType = null);
        Task<string?> GetComposition(string codename);
        Task<string?> GetSubpages(string url);
    }
}
