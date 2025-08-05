using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.WebAPI.DTOs.Stock;
using AutoMapper;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockLogic _stockLogic;
        private readonly IMapper _mapper;
        private readonly ILogger<StockController> _logger;

        public StockController(
            IStockLogic stockLogic,
            IMapper mapper,
            ILogger<StockController> logger)
        {
            _stockLogic = stockLogic;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockBalanceResponse>>> GetFiltered(
            [FromQuery] StockFilterRequest filter)
        {
            try
            {
                var balances = await _stockLogic.GetFilteredStockBalancesAsync(
                    filter.ResourceIds,
                    filter.UnitIds);

                var sortedBalances = balances
                    .OrderBy(b => b.Resource?.Name ?? "")
                    .ToList();

                var response = _mapper.Map<IEnumerable<StockBalanceResponse>>(sortedBalances);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке складских остатков");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}