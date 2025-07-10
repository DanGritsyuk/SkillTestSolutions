using CompanyDirectory.BLL.Logic.Contracts;
using CompanyDirectory.Common.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDirectory.PL.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(
            IDepartmentService service,
            ILogger<DepartmentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _service.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetDepartmentByIdAsync(id);
            return department != null ? Ok(department) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department department)
        {
            try
            {
                var id = await _service.CreateDepartmentAsync(department);
                return CreatedAtAction(nameof(GetById), new { id }, department);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department department)
        {
            if (id != department.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var success = await _service.UpdateDepartmentAsync(department);
                return success ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteDepartmentAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
