using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetCategoryByIdQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}
