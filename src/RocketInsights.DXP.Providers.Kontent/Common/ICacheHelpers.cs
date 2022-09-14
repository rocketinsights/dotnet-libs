using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.Models;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.Common
{
    public interface ICacheHelpers
    {
        Task<CodenameDetails> GetCodename(string url);
        Task SaveCodename(string url, string? codename);
        Task<Composition?> GetComposition(string? codename);
        Task SaveComposition(string? codename, Composition? composition);
    }
}
