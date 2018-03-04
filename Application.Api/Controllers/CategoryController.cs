using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
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
        
        public IQueryable<Category> Get(ODataQueryOptions<Category> queryOptions)
        {
            if (queryOptions == null)
                return _serviceManager.ProcessQueryAsync(new GetCategoriesQuery()).Result.AsQueryable();
            
            var result = _serviceManager.ProcessQueryAsync(ApplyODataQueryConditions(queryOptions, new GetCategoriesQuery())).Result;
            return result.AsQueryable();
        }
        
        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult> GetByIdAsync(int id)
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
