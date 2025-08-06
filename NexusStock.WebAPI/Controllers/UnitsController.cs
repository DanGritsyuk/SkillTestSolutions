using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Unit;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitLogic _unitLogic;
        private readonly ILogger<UnitsController> _logger;
        private readonly IMapper _mapper;

        public UnitsController(
            IUnitLogic unitLogic,
            ILogger<UnitsController> logger,
            IMapper mapper)
        {
            _unitLogic = unitLogic;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("get_all")]
        public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                IEnumerable<Unit> units;

                if (includeArchived)
                    units = await _unitLogic.GetAllUnitsAsync();
                else
                    units = await _unitLogic.GetAllActiveUnitsAsync();

                var response = _mapper.Map<IEnumerable<UnitResponse>>(units);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении единиц измерения");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitResponse>> GetById(int id)
        {
            try
            {
                var unit = await _unitLogic.GetUnitByIdAsync(id);

                if (unit == null)
                    return NotFound();

                return Ok(_mapper.Map<UnitResponse>(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении единицы измерения ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UnitCreateRequest request)
        {
            try
            {
                var unit = _mapper.Map<Unit>(request);
                await _unitLogic.CreateUnitAsync(unit);

                var response = _mapper.Map<UnitResponse>(unit);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = unit.Id },
                    response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании единицы измерения");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UnitUpdateRequest request)
        {
            try
            {
                var unit = _mapper.Map<Unit>(request);
                await _unitLogic.UpdateUnitAsync(unit);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении единицы измерения ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPatch("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] UnitStatusRequest request)
        {
            try
            {
                await _unitLogic.ToggleUnitStatusAsync(request.Id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при изменении статуса единицы измерения ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}