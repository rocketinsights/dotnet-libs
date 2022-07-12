using RocketInsights.DXP.Models;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Services
{
    public interface ILayoutService
    {
        Task<Composition> GetCompositionAsync();
    }
}