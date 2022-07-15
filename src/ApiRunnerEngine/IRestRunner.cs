using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiRunnerEngine
{
    public interface IRestRunner
    {
        Task<T?> Execute<T>(RestRequestProperties requestProperties) where T : class;
        Task<string?> Execute(RestRequestProperties requestProperties);
    }
}
