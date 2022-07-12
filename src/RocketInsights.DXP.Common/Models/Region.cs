namespace RocketInsights.DXP.Models
{
    public class Region : TemplatedModelBase
    {
        public IEnumerable<Region> Regions = new List<Region>();
        public IEnumerable<Fragment> Fragments = new List<Fragment>();
    }
}
