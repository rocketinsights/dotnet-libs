using RocketInsights.Common.Patterns;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Factories
{
    public class DefaultCompositionFactory : IFactory<Composition>
    {
        private IFactory<ILayoutService> LayoutServiceFactory { get; }

        public DefaultCompositionFactory(IFactory<ILayoutService> layoutServiceFactory)
        {
            LayoutServiceFactory = layoutServiceFactory;
        }

        public async Task<Composition> Create()
        {
            var layoutService = await LayoutServiceFactory.Create();

            return await layoutService.GetCompositionAsync();
        }
    }
}
