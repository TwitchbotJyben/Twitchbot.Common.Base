using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Twitchbot.Common.Base.Models;

namespace Twitchbot.Common.Base.Interfaces
{
    public interface IApiClient
    {
        Task<HttpResultModel<TOut>> PerformRequest<TOut>(string uri, HttpMethod method, object content = default, Dictionary<string, string> headers = default) where TOut : class;
    }
}