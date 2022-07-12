using RocketInsights.DXP.Models;

namespace RocketInsights.DXP.Services
{
    public interface IContentService
    {
        Task<Fragment> GetContent(Guid id);
    }
}