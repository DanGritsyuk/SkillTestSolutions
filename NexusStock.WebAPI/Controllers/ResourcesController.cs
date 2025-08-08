using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Resource;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// API версии 1.0 для управления ресурсами (товарами/материалами)
    /// </summary>
    /// <remarks>
    /// Все операции с ресурсами используют UTC время для любых временных меток.
    /// Сервер автоматически обрабатывает все даты в UTC формате.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceLogic _resourceLogic;
        private readonly IMapper _mapper;
        private readonly ILogger<ResourcesController> _logger;

        public ResourcesController(
            IResourceLogic resourceLogic,
            IMapper mapper,
            ILogger<ResourcesController> logger)
        {
            _resourceLogic = resourceLogic;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получение списка ресурсов
        /// </summary>
        /// <param name="includeArchived">Включать архивные ресурсы (true/false)</param>
        /// <response code="200">Успешно возвращен список ресурсов</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Возвращает:
        /// - Только активные ресурсы по умолчанию
        /// - Все ресурсы (включая архивные) при includeArchived=true
        /// - Все даты связанные с ресурсами представлены в UTC
        /// </remarks>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ResourceResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                var resources = includeArchived
                    ? await _resourceLogic.GetAllArchiveResourcesAsync()
                    : await _resourceLogic.GetAllActiveResourcesAsync();

                return Ok(_mapper.Map<IEnumerable<ResourceResponse>>(resources));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ресурсов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получение ресурса по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор ресурса</param>
        /// <response code="200">Успешно возвращен ресурс</response>
        /// <response code="404">Ресурс не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Возвращает полную информацию о ресурсе включая:
        /// - Базовые характеристики
        /// - Историю изменений (даты в UTC)
        /// - Статус (активный/архивный)
        /// </remarks>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ResourceResponse>> GetById(int id)
        {
            try
            {
                var resource = await _resourceLogic.GetResourceByIdAsync(id);
                return resource == null
                    ? NotFound()
                    : Ok(_mapper.Map<ResourceResponse>(resource));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении ресурса ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Создание нового ресурса
        /// </summary>
        /// <param name="request">Данные для создания ресурса</param>
        /// <response code="201">Ресурс успешно создан</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Особенности:
        /// - Дата создания устанавливается автоматически (UTC время сервера)
        /// - Все входящие даты должны быть в UTC формате
        /// - Ресурс создается в активном состоянии
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ResourceCreateRequest request)
        {
            try
            {
                var resource = _mapper.Map<Resource>(request);
                await _resourceLogic.CreateResourceAsync(resource);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = resource.Id },
                    _mapper.Map<ResourceResponse>(resource));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании ресурса");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновление существующего ресурса
        /// </summary>
        /// <param name="id">Идентификатор ресурса</param>
        /// <param name="request">Обновленные данные ресурса</param>
        /// <response code="204">Ресурс успешно обновлен</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Ресурс не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Особенности обновления:
        /// - Дата последнего обновления устанавливается автоматически (UTC)
        /// - Все временные метки должны передаваться в UTC
        /// - Нельзя обновить архивный ресурс
        /// </remarks>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResourceUpdateRequest request)
        {
            try
            {
                var resource = _mapper.Map<Resource>(request);
                resource.Id = id;
                await _resourceLogic.UpdateResourceAsync(resource);
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
                _logger.LogError(ex, $"Ошибка при обновлении ресурса ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Переключение статуса ресурса (активный/архивный)
        /// </summary>
        /// <param name="id">Идентификатор ресурса</param>
        /// <response code="204">Статус успешно изменен</response>
        /// <response code="404">Ресурс не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Особенности:
        /// - Архивация не удаляет ресурс, а помечает его как неактивный
        /// - Дата изменения статуса фиксируется в UTC времени
        /// - Архивные ресурсы не участвуют в основных операциях
        /// </remarks>
        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _resourceLogic.ToggleResourceStatusAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при изменении статуса ресурса ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}