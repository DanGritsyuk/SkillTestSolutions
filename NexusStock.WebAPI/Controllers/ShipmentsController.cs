using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Shipment;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentLogic _shipmentLogic;
        private readonly ILogger<ShipmentsController> _logger;
        private readonly IMapper _mapper;

        public ShipmentsController(
            IShipmentLogic shipmentLogic,
            ILogger<ShipmentsController> logger,
            IMapper mapper)
        {
            _shipmentLogic = shipmentLogic;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentResponse>>> GetFiltered(
            [FromQuery] ShipmentFilterRequest filter)
        {
            try
            {
                var shipments = await _shipmentLogic.GetFilteredShipmentsAsync(
                    filter.StartDate,
                    filter.EndDate,
                    filter.ClientIds,
                    filter.ResourceIds,
                    filter.UnitIds);

                var response = _mapper.Map<IEnumerable<ShipmentResponse>>(shipments);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке документов отгрузки");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentResponse>> GetById(int id)
        {
            try
            {
                var shipment = await _shipmentLogic.GetShipmentWithItemsAsync(id);

                if (shipment == null)
                    return NotFound();

                return Ok(_mapper.Map<ShipmentResponse>(shipment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении документа отгрузки ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShipmentCreateRequest request)
        {
            try
            {
                var shipment = _mapper.Map<ShipmentDocument>(request);
                await _shipmentLogic.CreateShipmentAsync(shipment);

                var response = _mapper.Map<ShipmentResponse>(shipment);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = shipment.Id },
                    response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании документа отгрузки");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ShipmentUpdateRequest request)
        {
            try
            {
                var shipment = _mapper.Map<ShipmentDocument>(request);
                await _shipmentLogic.UpdateShipmentAsync(shipment);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Документ подписан и не может быть изменен
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении документа отгрузки ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shipmentLogic.DeleteShipmentAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Документ подписан и не может быть удален
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении документа отгрузки ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPatch("sign")]
        public async Task<IActionResult> Sign([FromBody] ShipmentSignRequest request)
        {
            try
            {
                await _shipmentLogic.SignShipmentAsync(request.Id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                // Недостаточно товара на складе
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при подписании документа отгрузки ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        [HttpPatch("revoke")]
        public async Task<IActionResult> Revoke([FromBody] ShipmentSignRequest request)
        {
            try
            {
                await _shipmentLogic.RevokeShipmentAsync(request.Id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при отзыве документа отгрузки ID: {request.Id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}