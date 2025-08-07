using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Receipt;
using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ReceiptResponse>>> GetFiltered(
            [FromQuery] ReceiptFilterRequest filter)
        {
            try
            {
                var documents = await _receiptLogic.GetFilteredReceiptsAsync(
                    filter.StartDate,
                    filter.EndDate,
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