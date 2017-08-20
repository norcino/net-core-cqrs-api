﻿using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Service.Common;
using Service.Payment.Command;
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

        [HttpGet("{id}", Name = "GetPaymentById")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var result = await _serviceManager.ProcessQueryAsync(new GetPaymentByIdQuery(id));

            if (result == null)
            {
                return NotFound();
            }

            return new OkObjectResult(result);
        }

        public async Task<ActionResult> PostAsync([FromBody] Payment payment)
        {
            var result = await _serviceManager.ProcessCommandAsync<int>(new CreatePaymentCommand(payment));
            return new CreatedAtRouteResult("GetPaymentById", new { Id = result.Result }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] Payment payment)
        {
            var result = await _serviceManager.ProcessCommandAsync<int>(new UpdatePaymentCommand(id, payment));
            return new OkObjectResult(result);
        }
    }
}
