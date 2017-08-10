using System;
using System.Collections.Generic;
using System.Linq;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly HouseKeepingContext _context;

        public TransactionController(HouseKeepingContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            return _context.Transactions.ToList();
        }
        
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = _context.Transactions.SingleOrDefault(c => c.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}
