using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Common;
using Service.Transaction.Queries;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public TransactionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetTransactionByIdQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}
