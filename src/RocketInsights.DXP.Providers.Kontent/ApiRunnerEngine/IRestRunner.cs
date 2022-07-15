using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public interface IRestRunner
    {
        Task<T?> Execute<T>(RestRequestProperties requestProperties) where T : class;
        Task<string?> Execute(RestRequestProperties requestProperties);
    }
}
