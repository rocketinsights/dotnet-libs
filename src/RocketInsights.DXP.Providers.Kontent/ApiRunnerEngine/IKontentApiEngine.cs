using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public interface IKontentApiEngine
    {
        Task<string?> GetContentItem(string codename);
        Task<string?> GetComposition(Uri uri);
    }
}
