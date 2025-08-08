using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Unit;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// API версии 1.0 для управления единицами измерения
    /// </summary>
    /// <remarks>
    /// Предоставляет методы для работы с единицами измерения.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        /// <summary>
        /// Получает список единиц измерения
        /// </summary>
        /// <param name="includeArchived">
        /// Флаг включения архивных записей:
        /// - true: вернуть все архивные записи
        /// - false: вернуть только активные записи (по умолчанию)
        /// </param>
        /// <response code="200">Успешный возврат списка единиц измерения</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                IEnumerable<Unit> units = includeArchived
                    ? await _unitLogic.GetAllArchiveUnitsAsync()
                    : await _unitLogic.GetAllActiveUnitsAsync();

                var response = _mapper.Map<IEnumerable<UnitResponse>>(units);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении единиц измерения");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получает единицу измерения по ID
        /// </summary>
        /// <param name="id">Идентификатор единицы измерения</param>
        /// <response code="200">Единица измерения найдена</response>
        /// <response code="404">Единица измерения не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("get/{id}")]
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

        /// <summary>
        /// Создает новую единицу измерения
        /// </summary>
        /// <param name="request">Данные для создания единицы измерения</param>
        /// <response code="201">Единица измерения успешно создана</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="500">Ошибка сервера</response>
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

        /// <summary>
        /// Обновляет существующую единицу измерения
        /// </summary>
        /// <param name="id">Идентификатор единицы измерения</param>
        /// <param name="request">Данные для обновления</param>
        /// <response code="204">Единица измерения обновлена</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="404">Единица измерения не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UnitUpdateRequest request)
        {
            try
            {
                var unit = _mapper.Map<Unit>(request);
                unit.Id = id;
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

        /// <summary>
        /// Переключает статус активности единицы измерения
        /// </summary>
        /// <param name="id">Идентификатор единицы измерения</param>
        /// <remarks>
        /// Действие:
        /// - Для активной единицы: переводит в архив
        /// - Для архивной единицы: восстанавливает из архива
        /// </remarks>
        /// <response code="204">Статус успешно изменен</response>
        /// <response code="404">Единица измерения не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _unitLogic.ToggleUnitStatusAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при изменении статуса единицы измерения ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}