namespace RocketInsights.Common.Patterns
{
    public interface IFactory<T>
    {
        T Create();
    }
}
