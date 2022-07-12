using RocketInsights.DXP.Models;

namespace RocketInsights.DXP.Services
{
    public interface ILayoutService
    {
        Task<Composition> GetComposition();
    }
}