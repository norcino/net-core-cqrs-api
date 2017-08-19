using System.Threading.Tasks;
using Data.Entity;
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
        
        [HttpGet("{id}", Name = "GetById")]
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
            return new CreatedAtRouteResult("GetById", new { Id = result.Result }, result);
        }
    }
}
