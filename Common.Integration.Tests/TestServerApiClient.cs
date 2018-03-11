using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Common.IntegrationTests
{
    public class TestServerApiClient
    {
        private readonly HttpClient _client;

        public TestServerApiClient()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseEnvironment("Test");
            webHostBuilder.UseStartup<Startup>();
            var server = new TestServer(webHostBuilder);
            _client = server.CreateClient();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            return await _client.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        public async Task<T> GetContentAsync<T>(string url)
        {
            var response = await GetAsync(url);
            return response.To<T>();
        }
    }

    public static class TestServerApiClientExtensions
    {
        public static T To<T>(this HttpResponseMessage response)
        {
            var responseString = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}
