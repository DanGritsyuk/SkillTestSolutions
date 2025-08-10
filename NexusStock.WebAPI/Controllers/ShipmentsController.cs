using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Shipment;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// API версии 1.0 для управления документами отгрузки
    /// </summary>
    /// <remarks>
    /// Все операции с датами выполняются исключительно в формате UTC.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        /// <summary>
        /// Получает отфильтрованный список документов отгрузки
        /// </summary>
        /// <param name="filter">Параметры фильтрации</param>
        /// <remarks>
        /// Все даты автоматически конвертируются в UTC
        /// Сервер автоматически конвертирует входящие даты в UTC.
        /// </remarks>
        /// <response code="200">Успешный возврат списка отгрузок</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ShipmentResponse>>> GetFiltered(
            [FromQuery] ShipmentFilterRequest filter)
        {
            try
            {
                var shipments = await _shipmentLogic.GetFilteredShipmentsAsync(
                    filter.StartDate?.UtcDateTime,
                    filter.EndDate?.UtcDateTime,
                    filter.IsSigned,
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

        /// <summary>
        /// Получает документ отгрузки по ID
        /// </summary>
        /// <param name="id">Идентификатор документа отгрузки</param>
        /// <response code="200">Документ найден</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("get/{id}")]
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

        /// <summary>
        /// Создает новый документ отгрузки
        /// </summary>
        /// <param name="request">Данные для создания документа</param>
        /// <remarks>
        /// Временные метки автоматически конвертируются в UTC
        /// </remarks>
        /// <response code="201">Документ успешно создан</response>
        /// <response code="400">Ошибка валидации</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("create")]
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

        /// <summary>
        /// Обновляет существующий документ отгрузки
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <param name="request">Данные для обновления</param>
        /// <remarks>
        /// Ограничения:
        /// - Нельзя обновлять подписанные документы
        /// - Все даты автоматически конвертируются в UTC
        /// </remarks>
        /// <response code="204">Документ обновлен</response>
        /// <response code="400">Ошибка валидации или документ подписан</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShipmentUpdateRequest request)
        {
            try
            {
                var shipment = _mapper.Map<ShipmentDocument>(request);
                shipment.Id = id;
                await _shipmentLogic.UpdateShipmentAsync(shipment);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении документа отгрузки ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удаляет документ отгрузки
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <remarks>
        /// Ограничения:
        /// - Нельзя удалять подписанные документы
        /// </remarks>
        /// <response code="204">Документ удален</response>
        /// <response code="400">Документ подписан</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shipmentLogic.DeleteShipmentAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении документа отгрузки ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Подписывает документ отгрузки
        /// </summary>
        /// <param name="request">Данные для подписания</param>
        /// <remarks>
        /// Проверки при подписании:
        /// - Достаточность товара на складе
        /// - Документ должен существовать
        /// - Документ не должен быть уже подписан
        /// </remarks>
        /// <response code="204">Документ подписан</response>
        /// <response code="400">Недостаточно товара</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Ошибка сервера</response>
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

        /// <summary>
        /// Отзывает подпись документа отгрузки
        /// </summary>
        /// <param name="request">Данные для отзыва</param>
        /// <remarks>
        /// Требования:
        /// - Документ должен быть подписан
        /// - Документ должен существовать
        /// </remarks>
        /// <response code="204">Подпись отозвана</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Ошибка сервера</response>
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