using System;
using Common.IntegrationTests;
using Common.Tests.FluentAssertion;
using Data.Common.Testing.Builder;
using Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Api.IntegrationTests
{
    [TestClass]
    public class CategoryControllerTests : BaseApiIntegrationTests
    {
        private Persister<Category> _categoryPersister;
        private Builder<Category> _categoryBuilder;

        [TestInitialize]
        public void TestInitialize()
        {       
            _categoryPersister = new Persister<Category>(_context);
            _categoryBuilder = new Builder<Category>();
        }

        [TestCleanup]
        public void TestCleanup()
        {            
            _categoryPersister?.Dispose();        
        }

        #region GET
        [TestMethod]
        public async Task GET_support_orderBy_Name_descending()
        {
            const int expectedCategories = 5;
            _categoryPersister.Persist(expectedCategories, (c, i) =>
            {
                c.Active = 1 % 2 == 0;
                c.Name = (expectedCategories - i).ToString();
                c.Description = i.ToString();
            });

            var response = await _client.GetAsync("/api/category?$orderby=Name desc");
            Assert.That.IsOkHttpResponse(response);
            var categories = response.To<List<Category>>();
            for (var i = 0; i < expectedCategories - 1; i++)
            {
                Assert.IsTrue(categories[i].Id < categories[i + 1].Id);
                Assert.IsTrue(int.Parse(categories[i].Name) > int.Parse(categories[i + 1].Name));
            }
        }

        [TestMethod]
        public async Task GET_support_orderBy_Name_ascending()
        {
            const int expectedCategories = 5;
            _categoryPersister.Persist(expectedCategories, (c, i) =>
            {
                c.Active = 1 % 2 == 0;
                c.Name = (expectedCategories - i).ToString();
                c.Description = i.ToString();
            });

            var response = await _client.GetAsync("/api/category?$orderby=Name");
            Assert.That.IsOkHttpResponse(response);
            var categories = response.To<List<Category>>();
            for (var i = 0; i < expectedCategories - 1; i++)
            {
                Assert.IsTrue(categories[i].Id > categories[i + 1].Id);
                Assert.IsTrue(int.Parse(categories[i].Name) < int.Parse(categories[i + 1].Name));
            }
        }

        [TestMethod]
        public async Task GET_return_all_categories()
        {
            _categoryPersister.Persist(10, (c, i) =>
            {
                c.Active = 1 % 2 == 0;
                c.Name = (10 - i).ToString();
                c.Description = i.ToString();
            });

            var categories = await _client.GetAsync("/api/category");

            Assert.That.All(categories.To<List<Category>>()).HaveCount(10);
        }

        [TestMethod]
        public async Task GET_return_all_categories_limiting_to_the_first_100()
        {
            const int NumberOfCatetoriesToCreate = 110;

            _categoryPersister.Persist(NumberOfCatetoriesToCreate);

            var categories = await _client.GetAsync("/api/category");

            Assert.That.All(categories.To<List<Category>>()).HaveCount(MaxPageItemNumber);
        }

        [TestMethod]
        public async Task GET_support_orderBy_Id_ascending()
        {
            const int NumberOfTransactionsToCreate = 3;
            _categoryPersister.Persist(NumberOfTransactionsToCreate);

            var response = await _client.GetAsync("/api/category?&orderby=Id");
            var categories = response.To<List<Category>>();

            Assert.That.All(categories).HaveCount(NumberOfTransactionsToCreate);
            Assert.IsTrue(categories[0].Id < categories[1].Id &&
                          categories[1].Id < categories[2].Id);
        }

        [TestMethod]
        public async Task GET_support_orderBy_Id_descending()
        {
            const int NumberOfTransactionsToCreate = 3;
            _categoryPersister.Persist(NumberOfTransactionsToCreate);

            var response = await _client.GetAsync("/api/category?$orderby=Id desc");
            var categories = response.To<List<Category>>();

            Assert.That.All(categories).HaveCount(NumberOfTransactionsToCreate);
            Assert.IsTrue(categories[0].Id > categories[1].Id &&
                          categories[1].Id > categories[2].Id);
        }
        #endregion

        #region GET by ID
        [TestMethod]
        public async Task GET_byId_returns_404_when_id_does_not_exist()
        {
            var response = await _client.GetAsync("/api/category/1");
            Assert.That.IsNotFoundHttpResponse(response);
        }

        [TestMethod]
        public async Task GET_byId_returns_ok_200_when_entity_with_specified_Id_exists()
        {
            var category = _categoryPersister.Persist();
            var response = await _client.GetAsync($"/api/category/{category.Id}");
            Assert.That.IsOkHttpResponse(response);
        }

        [TestMethod]
        public async Task GET_byId_returns_correct_and_complete_entity_with_the_specified_Id()
        {
            var expectedCategory = _categoryPersister.Persist();
            var response = await _client.GetAsync($"/api/category/{expectedCategory.Id}");
            var category = response.To<Category>();
            Assert.IsNotNull(category);
            Assert.That.This(expectedCategory).HasSameProperties(category);
        }
        #endregion

        #region POST
        [TestMethod]
        public async Task POST_returns_201_passing_valid_entity()
        {
            var expectedCategory = _categoryBuilder.Build(c => c.Id = 0);
            var response = await _client.PostAsync<Category>("/api/category", expectedCategory);

            Assert.That.IsCreatedHttpResponse(response);
        }

        [TestMethod]
        public async Task POST_location_URI_to_access_the_created_entity()
        {
            var expectedCategory = _categoryBuilder.Build(c => c.Id = 0);
            var postResponse = await _client.PostAsync<Category>("/api/Category", expectedCategory);
            Assert.IsNotNull(postResponse.Headers.Location.AbsoluteUri, "Location should be set with the URL to the created object");
            Assert.IsTrue(postResponse.Headers.Location.AbsoluteUri.Contains("/api/Category/"), "Entity URI has targets the right position");
        }

        [TestMethod]
        public async Task POST_creates_valid_entity()
        {
            var expectedCategory = _categoryBuilder.Build(c => c.Id = 0);
            var postResponse = await _client.PostAsync<Category>("/api/Category", expectedCategory);
            var getResponse = await _client.GetAsync(postResponse.Headers.Location.AbsoluteUri);

            Assert.That.IsOkHttpResponse(getResponse);
            var category = getResponse.To<Category>();
            Assert.IsNotNull(category);
            Assert.That.This(expectedCategory).HasSameProperties(category, nameof(category.Id));
        }

        [TestMethod]
        public async Task POST_return_BadRequest_400_trying_to_create_entity_with_Id()
        {
            var expectedCategory = _categoryBuilder.Build();
            var postResponse = await _client.PostAsync<Category>("/api/Category", expectedCategory);
            Assert.That.IsBadRequestHttpResponse(postResponse);
        }
        #endregion
    }
}
