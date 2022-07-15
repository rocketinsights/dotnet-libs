using RocketInsights.Contextual.Models;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.Services
{
    public interface IContextService
    {
        Task GenerateAndSetContext();
        bool TryGetContext(out Context context);
    }
}