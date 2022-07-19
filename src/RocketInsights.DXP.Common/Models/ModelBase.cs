using RocketInsights.Common.Models;
using System;

namespace RocketInsights.DXP.Models
{
    public class ModelBase
    {
        public string Id { get; set; } = string.Empty;
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public Content Content { get; set; } = new Content();
    }

    public class TemplatedModelBase : ModelBase
    {
        public Template Template { get; set; } = new Template();
    }
}
