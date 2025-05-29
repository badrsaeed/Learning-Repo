using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViralWave.Application.Helpers
{
    public class HttpClientHelper
    {
        private static HttpClient _httpClient;
        static HttpClientHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 5, 0);
        }
        /// <summary>
        /// Send any Http Requests.
        /// </summary>
        /// <param name="method">Request Method Type</param>
        /// <param name="Url">Full Request URL</param>
        /// <param name="headers">Headers of the request</param>
        /// <returns>String Of Response Content</returns>
        public async static Task<HttpResponseMessage> SendAsync(HttpMethod method, string Url, Dictionary<string, string>? headers = null)
        {
            var request = new HttpRequestMessage(method, Url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            var response = await _httpClient.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            // var content = await response.Content.ReadAsStringAsync();
            return response;
        }
        /// <summary>
        /// Send any Http Requests.
        /// </summary>
        /// <param name="method">Request Method Type</param>
        /// <param name="Url">Full Request URL</param>
        /// <param name="headers">Headers of the request</param>
        /// <param name="requestContent">Content payload</param>
        /// <returns></returns>
        public async static Task<HttpResponseMessage> SendAsync(HttpMethod method, string Url, StringContent requestContent, Dictionary<string, string>? headers = null)
        {
            var request = new HttpRequestMessage(method, Url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            request.Content = requestContent;
            var response = await _httpClient.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            //var content = await response.Content.ReadAsStringAsync();
            return response;
        }
    }
}
