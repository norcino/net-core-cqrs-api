using System;
using System.Collections.Generic;
using System.Linq;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly HouseKeepingContext _context;

        public CategoryController(HouseKeepingContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return _context.Categories.ToList();
        }
        
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}
