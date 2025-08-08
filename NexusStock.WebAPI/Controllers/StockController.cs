using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.WebAPI.DTOs.Stock;
using AutoMapper;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// API версии 1.0 для управления складскими остатками
    /// </summary>
    /// <remarks>
    /// Временные метки возвращаются в UTC формате
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        /// <summary>
        /// Получение складских остатков с фильтрацией
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/v1.0/stock?resourceIds=1,2,3&amp;unitIds=5,6
        /// 
        /// Возвращаемые данные:
        /// - Все даты представлены в UTC формате
        /// - Остатки отсортированы по наименованию ресурса
        /// </remarks>
        /// <param name="filter">Параметры фильтрации (resourceIds, unitIds)</param>
        /// <response code="200">Возвращает список складских остатков</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("get")]
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