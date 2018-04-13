﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Category.Command;
using Service.Category.Query;
using Service.Common;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : BaseController
    {
        private readonly IServiceManager _serviceManager;

        public CategoryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public Task<List<Category>> Get(ODataQueryOptions<Category> queryOptions)
        {
            var query = ApplyODataQueryConditions<Category, GetCategoriesQuery>(queryOptions, new GetCategoriesQuery());
            return _serviceManager.ProcessQueryAsync(query);
        }
        
        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetCategoryByIdQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] Category category)
        {
            var result = await _serviceManager.ProcessCommandAsync<int>(new CreateCategoryCommand(category));

            if (result.Successful)
            {
                return new CreatedAtRouteResult("GetCategoryById", new {Id = result.Result}, result);
            }

            return StatusCode(StatusCodes.Status400BadRequest, result);
        }
    }
}
