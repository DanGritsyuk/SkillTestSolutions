using CompanyDirectory.BLL.Logic.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDirectory.PL.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService service,
            ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("high-salary")]
        public async Task<IActionResult> GetHighSalary()
        {
            var employees = await _service.GetHighSalaryEmployeesAsync();
            return Ok(employees);
        }

        [HttpDelete("retired")]
        public async Task<IActionResult> DeleteRetired()
        {
            var count = await _service.DeleteRetiredEmployeesAsync();
            return Ok($"Deleted {count} retired employees");
        }

        [HttpPut("adjust-salaries")]
        public async Task<IActionResult> AdjustSalaries()
        {
            var count = await _service.RaiseLowSalariesAsync();
            return Ok($"Adjusted salaries for {count} employees");
        }
    }
}
