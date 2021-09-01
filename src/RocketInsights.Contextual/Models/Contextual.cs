namespace RocketInsights.Contextual.Models
{
    public class Contextual<T>
    {
        public Context Context { get; }
        public T Payload { get; }

        public Contextual(Context context, T payload)
        {
            Context = context;
            Payload = payload;
        }
    }
}