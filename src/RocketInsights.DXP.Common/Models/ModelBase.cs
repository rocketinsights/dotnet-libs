namespace RocketInsights.DXP.Models
{
    public class ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public Content Content { get; set; } = new Content();
    }

    public class TemplatedModelBase : ModelBase
    {
        public Template Template { get; set; } = new Template();
    }
}
