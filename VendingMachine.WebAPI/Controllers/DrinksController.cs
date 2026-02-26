using Microsoft.AspNetCore.Mvc;
using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.Common.Entities;

namespace VendingMachine.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DrinksController : ControllerBase
    {
        private readonly IDrinkLogic _drinkLogic;

        public DrinksController(IDrinkLogic drinkLogic)
        {
            _drinkLogic = drinkLogic;
        }

        [HttpGet]
        public IAsyncEnumerable<Drink> GetAllDrinks() =>
            _drinkLogic.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Drink>> GetDrinkById(int id)
        {
            var drink = await _drinkLogic.GetByIdAsync(id);
            return drink is null ? NotFound() : Ok(drink);
        }

        [HttpGet("brand/{id}")]
        public async Task<IEnumerable<Drink>> GetAllByBrand(int id) =>
            await _drinkLogic.GetAllByBrandAsync(id);

        [HttpPost]
        public async Task<IActionResult> CreateDrink([FromBody] Drink drink)
        {
            await _drinkLogic.AddAsync(drink);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDrink(int id, [FromBody] Drink drink)
        {
            drink.ItemId = id;
            await _drinkLogic.UpdateAsync(drink);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrink(int id)
        {
            await _drinkLogic.DeleteAsync(id);
            return NoContent();
        }
    }
}
