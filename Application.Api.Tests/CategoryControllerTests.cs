using System.Collections.Generic;
using Application.Api.Controllers;
using Common.Tests;
using Data.Common.Testing.Builder;
using Data.Entity;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Category.Command;
using Service.Category.Query;
using Service.Common;

namespace Application.Api.Tests
{
    [TestClass]
    public class CategoryControllerTests
    {
        private CategoryController _controller;
        private Mock<IServiceManager> _serviceManagerMock;
        private Builder<Category> _builder;

        [TestInitialize]
        public void TestInitialize()
        {
            _builder = new Builder<Category>();
            _serviceManagerMock = new Mock<IServiceManager>();
            _controller = new CategoryController(_serviceManagerMock.Object);
        }

//        public Task<List<Category>> Get(ODataQueryOptions<Category> queryOptions)
//        {
//            var query = ApplyODataQueryConditions<Category, GetCategoriesQuery>(queryOptions, new GetCategoriesQuery());
//            return _serviceManager.ProcessQueryAsync(query);
//        }

        [TestMethod]
        public void Get_invokes_ProcessQueryAsync_on_ServiceManager_passing_GetCategoriesQuery_with_correct_data()
        {
            ODataQueryOptions<Category> queryOptions = new ODataQueryOptions<Category>(
                new ODataQueryContext(new EdmModel(), typeof(Category), new ODataPath()),
                new DefaultHttpRequest(new DefaultHttpContext()));
            
            var categoryId = AnonymousData.Int();
            var response = _controller.Get(categoryId);
            _serviceManagerMock.Verify(sm => sm.ProcessQueryAsync(It.Is<GetCategoriesQuery>(c =>
                c.Top != null
            )), Times.Once);
        }

        [TestMethod]
        public void GetById_invokes_ProcessQueryAsync_on_ServiceManager_passing_GetCategoryByIdQuery_with_correct_data()
        {
            var categoryId = AnonymousData.Int();
            var response = _controller.Get(categoryId);
            _serviceManagerMock.Verify(sm => sm.ProcessQueryAsync(It.Is<GetCategoryByIdQuery>(c =>
                c.CategoryId == categoryId
            )), Times.Once);
        }
        
        [TestMethod]
        public void Post_invokes_ProcessCommandAsync_on_ServiceManager_passing_CreateCategoryCommand_with_correct_data()
        {
            var category = _builder.Build(c => c.Id = 0);
            var response = _controller.PostAsync(category);
            _serviceManagerMock.Verify(sm => sm.ProcessCommandAsync<int>(It.Is<CreateCategoryCommand>(c =>
                c.Category.Description == category.Description &&
                c.Category.Name == category.Name &&
                c.Category.Active == category.Active
            )), Times.Once);
        }
    }
}
