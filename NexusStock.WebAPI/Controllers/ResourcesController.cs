using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Resource;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ResourceResponse>>> GetAll(bool includeArchived = false)
        {
            try
            {
                var resources = includeArchived
                    ? await _resourceLogic.GetAllResourcesAsync()
                    : await _resourceLogic.GetAllActiveResourcesAsync();

                return Ok(_mapper.Map<IEnumerable<ResourceResponse>>(resources));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ресурсов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

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

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResourceUpdateRequest request)
        {
            try
            {
                var resource = _mapper.Map<Resource>(request);
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

        [HttpPatch("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] ResourceStatusRequest request)
        {
            try
            {
                await _resourceLogic.ToggleResourceStatusAsync(request.Id);
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