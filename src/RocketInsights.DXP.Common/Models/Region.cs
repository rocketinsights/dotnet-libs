using System.Collections.Generic;

namespace RocketInsights.DXP.Models
{
    public class Region : TemplatedModelBase
    {
        public IEnumerable<Region> Regions { get; set; } = new List<Region>();
        public IEnumerable<Fragment> Fragments { get; set; } = new List<Fragment>();
    }
}
