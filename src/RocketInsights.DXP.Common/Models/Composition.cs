using System.Collections.Generic;

namespace RocketInsights.DXP.Models
{
    public class Composition : TemplatedModelBase
    {
        public IEnumerable<Region> Regions { get; set; } = new List<Region>();

    }
}