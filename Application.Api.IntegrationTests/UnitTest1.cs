using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Data.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Application.Api.IntegrationTests
{
    public class ApiClient {
        private TestServer _server;
        private HttpClient _client;

        public ApiClient()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseEnvironment("Test");
            webHostBuilder.UseStartup<Startup>();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
        }

        public async Task PostAsync<T>(string url, T entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, content);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync("/api/transaction?$take=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }

    [TestClass]
    public class UnitTest1
    {
        private ApiClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            _client = new ApiClient();
            var context = TestDataConfiguration.GetContex();
            var transaction = new Transaction
            {
                Credit = 1,
                Description = "A",
                Recorded = DateTime.Now
            };
            context.Transactions.Add(transaction);
            context.SaveChanges();

            var ooo = context.Transactions.ToList();
        }
        
        [TestMethod]
        public async Task TestMethod1()
        {
            await _client.PostAsync<Category>("/api/category", new Category{ Active = true, Description = "D", Name = "N"});

            var response = await _client.GetAsync<List<Category>>("/api/category?$take=1");
            
            Assert.AreEqual(1,1);
        }
    }
}
