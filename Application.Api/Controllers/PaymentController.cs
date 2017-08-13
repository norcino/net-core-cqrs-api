using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Common;
using Service.Payment.Query;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public PaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetPaymentByIdQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }
    }
}
