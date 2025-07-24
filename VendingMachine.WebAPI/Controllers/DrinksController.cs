using Microsoft.AspNetCore.Mvc;
using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.Common.Entities;

namespace VendingMachine.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DrinksController : ControllerBase
    {
        private readonly IDrinkLogic _drinkLogic;

        public DrinksController(IDrinkLogic drinkLogic, ILogger<DrinksController> logger)
        {
            _drinkLogic = drinkLogic;
            _logger = logger;
        }

        [HttpGet]
        [Route("getAll")]
        public IAsyncEnumerable<Drink> GetAllRecordsAsync() =>
            _drinkLogic.GetAllDrinksAsync();

        [HttpGet]
        [Route("getById")]
        public async Task<Drink?> GetDrinkByIdAsync(Guid id) =>
            await _drinkLogic.GetDrinkByIdAsync(id);

        [HttpGet]
        [Route("GetAllByBrand")]
        public async Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId) =>
            await _drinkLogic.GetAllByBrandAsync(brandId);
    }
}
