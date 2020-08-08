using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twitchbot.Base.Models;

namespace Twitchbot.Base.Client
{
    public class ClientBase
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly ILogger<ClientBase> _logger;
        private readonly IStringLocalizer<ClientBase> _localizer;

        public ClientBase(ILogger<ClientBase> logger, IStringLocalizer<ClientBase> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<HttpResultModel<TOut>> PerformRequest<TIn, TOut>(string uri, HttpMethod method, TIn content = default, Dictionary<string, string> headers = default) where TOut : class where TIn : class
        {
            _logger.LogInformation("GetRequest. Uri: {0}", uri);

            try
            {
                SetHeaders(headers);

                HttpResponseMessage response = new HttpResponseMessage();

                var serialized = content is null ? null : new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

                _logger.LogInformation("Body : {0}", serialized);

                response = await Execute(uri, method, response, serialized);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Result : {0}", responseBody);

                var resultModel = JsonConvert.DeserializeObject<TOut>(responseBody);

                return new HttpResultModel<TOut>
                {
                    Model = resultModel,
                    Result = true
                };
            }
            catch (HttpRequestException e)
            {
                return new HttpResultModel<TOut>
                {
                    Result = false,
                    ErrorMessage = e is HttpRequestException ? e.Message : _localizer["Une erreur innatendue s'est produite."]
                };
            }
        }

        private static async Task<HttpResponseMessage> Execute(string uri, HttpMethod method, HttpResponseMessage response, StringContent serialized)
        {
            switch (method)
            {
                case HttpMethod m when m == HttpMethod.Get:
                    response = await _client.GetAsync(uri);
                    break;
                case HttpMethod m when m == HttpMethod.Post:
                    response = await _client.PostAsync(uri, serialized);
                    break;
                case HttpMethod m when m == HttpMethod.Put:
                    response = await _client.PutAsync(uri, serialized);
                    break;
                case HttpMethod m when m == HttpMethod.Delete:
                    response = await _client.DeleteAsync(uri);
                    break;
                default:
                    break;
            }

            return response;
        }

        private void SetHeaders(Dictionary<string, string> headers)
        {
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers is null) return;

            foreach (KeyValuePair<string, string> kvp in headers)
            {
                _logger.LogInformation("Headers. Key: {0}, Value: {1}", kvp.Key, kvp.Value);

                if (kvp.Key == "Authorization")
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", kvp.Value);
                }
                else
                {
                    _client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}