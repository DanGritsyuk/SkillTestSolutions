using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Client;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                IEnumerable<Client> clients = includeArchived
                    ? await _clientLogic.GetAllClientsAsync()
                    : await _clientLogic.GetAllActiveClientsAsync();

                return Ok(_mapper.Map<IEnumerable<ClientResponse>>(clients));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиентов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("{id}")]
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

        [HttpPost]
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ClientUpdateRequest request)
        {
            try
            {
                var client = _mapper.Map<Client>(request);
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
                _logger.LogError(ex, $"Ошибка при обновлении клиента ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPatch("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] ClientStatusRequest request)
        {
            try
            {
                await _clientLogic.ToggleClientStatusAsync(request.Id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при изменении статуса клиента ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}