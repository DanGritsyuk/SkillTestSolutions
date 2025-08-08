using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Receipt;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления документами поступления товаров
    /// </summary>
    /// <remarks>
    /// Все операции с датами выполняются исключительно в формате UTC.
    /// Сервер автоматически конвертирует входящие даты в UTC.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptLogic _receiptLogic;
        private readonly IMapper _mapper;
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(
            IReceiptLogic receiptLogic,
            IMapper mapper,
            ILogger<ReceiptsController> logger)
        {
            _receiptLogic = receiptLogic;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получение отфильтрованных документов поступления
        /// </summary>
        /// <param name="filter">Параметры фильтрации</param>
        /// <response code="200">Возвращает список документов поступления</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/v1.0/receipts?startDate=2023-01-01T00:00:00Z&amp;endDate=2023-12-31T23:59:59Z
        /// 
        /// Особенности:
        /// - Все даты автоматически конвертируются в UTC
        /// - Если временная зона не указана, используется UTC
        /// - Пустые значения дат игнорируются при фильтрации
        /// </remarks>
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ReceiptResponse>>> GetFiltered(
            [FromQuery] ReceiptFilterRequest filter)
        {
            try
            {
                var documents = await _receiptLogic.GetFilteredReceiptsAsync(
                    filter.StartDate?.UtcDateTime,
                    filter.EndDate?.UtcDateTime,
                    filter.DocumentIds,
                    filter.ResourceIds,
                    filter.UnitIds);

                return Ok(_mapper.Map<IEnumerable<ReceiptResponse>>(documents));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке документов поступления");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получение документа поступления по ID
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <response code="200">Успешно возвращен документ</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Возвращает документ со всеми связанными позициями.
        /// Все даты в ответе представлены в UTC формате.
        /// </remarks>
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ReceiptResponse>> GetById(int id)
        {
            try
            {
                var document = await _receiptLogic.GetReceiptWithItemsAsync(id);
                return document == null
                    ? NotFound()
                    : Ok(_mapper.Map<ReceiptResponse>(document));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении документа поступления ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Создание нового документа поступления
        /// </summary>
        /// <param name="request">Данные для создания документа</param>
        /// <response code="201">Документ успешно создан</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Важно:
        /// - Все даты должны быть переданы в UTC формате
        /// - Если дата указана без временной зоны, она будет интерпретирована как UTC
        /// - Сервер автоматически устанавливает метку времени создания в UTC
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ReceiptCreateRequest request)
        {
            try
            {
                var document = _mapper.Map<ReceiptDocument>(request);
                await _receiptLogic.CreateReceiptAsync(document);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = document.Id },
                    _mapper.Map<ReceiptResponse>(document));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании документа поступления");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновление существующего документа поступления
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <param name="request">Обновленные данные документа</param>
        /// <response code="204">Документ успешно обновлен</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Особенности:
        /// - Все даты автоматически конвертируются в UTC
        /// - Метка времени последнего обновления устанавливается в UTC
        /// - Документы в подписанном состоянии не могут быть изменены
        /// </remarks>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReceiptUpdateRequest request)
        {
            try
            {
                var document = _mapper.Map<ReceiptDocument>(request);
                document.Id = id;
                await _receiptLogic.UpdateReceiptAsync(document);
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
                _logger.LogError(ex, $"Ошибка при обновлении документа поступления ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удаление документа поступления
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <response code="204">Документ успешно удален</response>
        /// <response code="404">Документ не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <remarks>
        /// Ограничения:
        /// - Подписанные документы не могут быть удалены
        /// - Удаление затрагивает все связанные позиции документа
        /// - Операция выполняется в UTC времени сервера
        /// </remarks>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _receiptLogic.DeleteReceiptAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении документа поступления ID: {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}