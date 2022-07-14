using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class KontentService : ILayoutService, IContentService
    {
        public Task<Composition> GetCompositionAsync()
        {
            var composition = new Composition()
            {
                Name = "This came from a Kontent provider."
            };

            return Task.FromResult(composition);
        }

        public Task<Fragment> GetFragmentAsync(string id)
        {
            var fragment = new Fragment()
            {
                Id = id,
                Name = "This came from a Kontent provider"
            };

            return Task.FromResult(fragment);
        }
    }
}
