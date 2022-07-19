using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Contentful.Proxy
{
    public interface IContentfulProxy
    {
        Task<Entry<T>> GetEntryAsync<T>(string entryId);
    }
}
