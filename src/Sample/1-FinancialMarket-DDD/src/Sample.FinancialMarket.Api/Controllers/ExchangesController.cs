using Asp.Versioning;
using Foundry.Api.BuildingBlocks.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Exchanges.Services;

namespace Sample.FinancialMarket.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/exchanges")]
    public class ExchangesController : BaseApiController
    {
        private readonly IExchangeService _exchangeService;

        public ExchangesController(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return HandleResult(await _exchangeService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRequest request)
        {
            return HandleResult(await _exchangeService.CreateAsync(request));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExchangeRequest request)
        {
            return HandleResult(await _exchangeService.UpdateAsync(id, request));
        }
    }
}