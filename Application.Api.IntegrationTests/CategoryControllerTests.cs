using System;
using Common.IntegrationTests;
using Common.Tests;
using Data.Common.Testing.Builder;
using Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Api.IntegrationTests
{
    [TestClass]
    public class CategoryControllerTests
    {
        private TestServerApiClient _client;
        private Persister<Category> _categoryPersister;
        private Builder<Category> _categoryBuilder;
        private IHouseKeeperContext _context;
        protected IDbContextTransaction Transaction;

        [TestInitialize]
        public void TestInitialize()
        {
            _client = new TestServerApiClient();
            _context = TestDataConfiguration.GetContext();
            _categoryPersister = new Persister<Category>(_context);
            _categoryBuilder = new Builder<Category>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
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

            var response = await _client.GetAsync("/api/category?&orderby=Name desc");
            Assert.That.IsOkHttpResponse(response);
            var categories = response.To<List<Category>>();
            for(var i = 0 ; i < expectedCategories - 1 ; i ++)
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

            var response = await _client.GetAsync("/api/category?&orderby=Name");
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

            var categories = _client.GetAsync("/api/category").Result.To<List<Category>>();

            Assert.That.HasCountOf(10, categories);
        }

        [TestMethod]
        public async Task GET_return_all_categories_limiting_to_the_first_100()
        {
            _categoryPersister.Persist(110, (c, i) =>
            {
                c.Active = 1 % 2 == 0;
                c.Name = (10 - i).ToString();
                c.Description = i.ToString();
            });

            var categories = _client.GetAsync("/api/category").Result.To<List<Category>>();

            Assert.That.HasCountOf(100, categories);
        }

        [TestMethod]
        public async Task GET_support_orderBy_Id()
        {
            _categoryPersister.Persist(3, (c, i) =>
            {
                c.Active = 1 % 2 == 0;
                c.Name = (10 - i).ToString();
                c.Description = i.ToString();
            });

            var response = await _client.GetAsync("/api/category?&orderby=Id");
            var categories = response.To<List<Category>>();
            
            Assert.That.HasCountOf(3, categories);
            Assert.IsTrue(categories[0].Id  < categories[1].Id &&
                          categories[1].Id < categories[2].Id);

            response = await _client.GetAsync("/api/category?$orderby=Id desc");
            categories = response.To<List<Category>>();
            
            Assert.That.HasCountOf(3, categories);
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
            Assert.That.HaveSameProperties(expectedCategory, category);
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
            Assert.That.HaveSameProperties(expectedCategory, category, nameof(category.Id));
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
