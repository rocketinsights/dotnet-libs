using RocketInsights.Contextual.Models;

namespace RocketInsights.Contextual.Services
{
    public interface IContextService
    {
        void GenerateAndSetContext();
        bool TryGetContext(out Context context);
    }
}