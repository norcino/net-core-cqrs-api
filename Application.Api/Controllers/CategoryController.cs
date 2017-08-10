using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Service.Category.Queries;
using Service.Common;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly IHouseKeeperContext _context;
        private readonly IServiceManager _serviceManager;

        public CategoryController(IServiceManager serviceManager, IHouseKeeperContext context)
        {
            _context = context;
            _serviceManager = serviceManager;
        }
        
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _context.Categories.ToList();
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
