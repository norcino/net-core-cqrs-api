﻿using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Category.Command;
using Service.Category.Query;
using Service.Common;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public CategoryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetCategoriesQuery());
            return new OkObjectResult(result);
        }
        
        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult> GetAsync(int id)
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

            return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
        }
    }
}
