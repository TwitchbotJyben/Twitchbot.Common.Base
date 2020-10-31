using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twitchbot.Common.Base.Interfaces;
using Twitchbot.Common.Base.Models;

namespace Twitchbot.Common.Base.Client
{
    /// <summary>
    /// Provides method to perform an http request.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly IStringLocalizer<ApiClient> _localizer;
        private List<string> ListAuthHeaders { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClient"/>.
        /// </summary>
        /// <param name="logger">The instance of <see cref="ILogger{TCategoryName}"/>.</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer{T}"/></param>
        public ApiClient(ILogger<ApiClient> logger, IStringLocalizer<ApiClient> localizer)
        {
            _logger = logger;
            _localizer = localizer;
            _client = new HttpClient();
            ListAuthHeaders = new List<string>()
            {
                "OAuth",
                "Bearer"
            };
        }

        /// <summary>
        /// Performs an http request.
        /// </summary>
        /// <typeparam name="TOut">The model result.</typeparam>
        /// <param name="uri">The uri with the endpoint.</param>
        /// <param name="method">The <see cref="HttpMethod"/>.</param>
        /// <param name="content">The body.</param>
        /// <param name="headers">All the headers.</param>
        /// <returns>The result with the model, errors or business message.</returns>
        public async Task<HttpResultModel<TOut>> PerformRequest<TOut>(string uri, HttpMethod method, object content = default, Dictionary<string, string> headers = default) where TOut : class
        {
            _logger.LogInformation("Request. Uri: {0}", uri);

            try
            {
                SetHeaders(headers);

                HttpResponseMessage response = new HttpResponseMessage();

                if (content is StringContent stringContent)
                {
                    _logger.LogInformation("Body : {0}", await stringContent.ReadAsStringAsync());
                    response = await Execute(uri, method, response, stringContent);
                }
                else if (content is FormUrlEncodedContent formUrlEncodedContent)
                {
                    _logger.LogInformation("Body : {0}", await formUrlEncodedContent.ReadAsStringAsync());
                    response = await Execute(uri, method, response, formUrlEncodedContent);
                }
                else
                {
                    response = await Execute(uri, method, response, stringContent : null);
                }

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
                _logger.LogError("Exception: {0}", e);

                return new HttpResultModel<TOut>
                {
                    Result = false,
                    ErrorMessage = e is HttpRequestException ? e.Message : _localizer["Une erreur innatendue s'est produite."]
                };
            }
        }

        private async Task<HttpResponseMessage> Execute(string uri, HttpMethod method, HttpResponseMessage response, StringContent stringContent)
        {
            switch (method)
            {
                case HttpMethod m when m == HttpMethod.Get:
                    response = await _client.GetAsync(uri);
                    break;
                case HttpMethod m when m == HttpMethod.Post:
                    response = await _client.PostAsync(uri, stringContent);
                    break;
                case HttpMethod m when m == HttpMethod.Put:
                    response = await _client.PutAsync(uri, stringContent);
                    break;
                case HttpMethod m when m == HttpMethod.Delete:
                    response = await _client.DeleteAsync(uri);
                    break;
                default:
                    break;
            }

            return response;
        }

        private async Task<HttpResponseMessage> Execute(string uri, HttpMethod method, HttpResponseMessage response, FormUrlEncodedContent formUrlEncodedContent)
        {
            switch (method)
            {
                case HttpMethod m when m == HttpMethod.Get:
                    response = await _client.GetAsync(uri);
                    break;
                case HttpMethod m when m == HttpMethod.Post:
                    response = await _client.PostAsync(uri, formUrlEncodedContent);
                    break;
                case HttpMethod m when m == HttpMethod.Put:
                    response = await _client.PutAsync(uri, formUrlEncodedContent);
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

                if (ListAuthHeaders.Contains(kvp.Key))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(kvp.Key, kvp.Value);
                }
                else
                {
                    _client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}