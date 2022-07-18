using RocketInsights.DXP.Models;
using System;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Services
{
    public interface IContentService
    {
        Task<Fragment> GetFragmentAsync(string id, string codename);
    }
}