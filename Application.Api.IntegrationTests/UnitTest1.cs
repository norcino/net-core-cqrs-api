using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Api.IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        private TestServerApiClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            _client = new TestServerApiClient();
            var context = TestDataConfiguration.GetContex();
            var category = new Category
            {
                Active = true,
                Description = "D",
                Name = "N"
            };
            context.Categories.Add(category);
            context.SaveChanges();
            var transaction = new Transaction
            {
                CategoryId = category.Id,
                Credit = 1,
                Description = "A",
                Recorded = DateTime.Now
            };
            context.Transactions.Add(transaction);
            context.SaveChanges();
        }
        
        [TestMethod]
        public async Task TestMethod1()
        {
            await _client.PostAsync<Category>("/api/category", new Category{ Active = true, Description = "D", Name = "N"});

            var response = await _client.GetAsync("/api/category?$take=10");
            var content = response.To<List<Category>>();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, content.Count);
        }
    }
}
