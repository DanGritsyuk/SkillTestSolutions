using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Client;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// API версии 1.0 для управления клиентами
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientLogic _clientLogic;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IClientLogic clientLogic,
            IMapper mapper,
            ILogger<ClientsController> logger)
        {
            _clientLogic = clientLogic;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        /// <param name="includeArchived">Включить архивные записи (true/false)</param>
        /// <response code="200">Успешно возвращен список клиентов</response>
        /// <response code="500">Ошибка сервера</response>
        /// <remarks>
        /// Сервер работает исключительно с UTC временем. Все даты возвращаются в UTC формате.
        /// </remarks>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                IEnumerable<Client> clients = includeArchived
                    ? await _clientLogic.GetAllArchiveClientsAsync()
                    : await _clientLogic.GetAllActiveClientsAsync();

                return Ok(_mapper.Map<IEnumerable<ClientResponse>>(clients));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиентов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получение клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <response code="200">Успешно возвращен клиент</response>
        /// <response code="404">Клиент не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ClientResponse>> GetById(int id)
        {
            try
            {
                var client = await _clientLogic.GetClientByIdAsync(id);
                return client == null
                    ? NotFound()
                    : Ok(_mapper.Map<ClientResponse>(client));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении клиента ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Создание нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <response code="201">Клиент успешно создан</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="500">Ошибка сервера</response>
        /// <remarks>
        /// Даты должны передаваться в UTC формате. Сервер автоматически конвертирует все даты в UTC.
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ClientCreateRequest request)
        {
            try
            {
                var client = _mapper.Map<Client>(request);
                await _clientLogic.CreateClientAsync(client);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = client.Id },
                    _mapper.Map<ClientResponse>(client));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании клиента");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновление данных клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="request">Обновленные данные клиента</param>
        /// <response code="204">Данные успешно обновлены</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Клиент не найден</response>
        /// <response code="500">Ошибка сервера</response>
        /// <remarks>
        /// Все временные метки автоматически конвертируются в UTC при обработке на сервере.
        /// </remarks>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientUpdateRequest request)
        {
            try
            {
                var client = _mapper.Map<Client>(request);
                client.Id = id;
                await _clientLogic.UpdateClientAsync(client);
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
                _logger.LogError(ex, $"Ошибка при обновлении клиента ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Переключение статуса клиента (активный/архивный)
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <response code="204">Статус успешно изменен</response>
        /// <response code="404">Клиент не найден</response>
        /// <response code="500">Ошибка сервера</response>
        /// <remarks>
        /// Архивация не удаляет клиента, а только помечает его как неактивный.
        /// Последнее изменение даты автоматически устанавливается в UTC времени сервера.
        /// </remarks>
        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _clientLogic.ToggleClientStatusAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при изменении статуса клиента ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}