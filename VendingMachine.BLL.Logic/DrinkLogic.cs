using Microsoft.Extensions.Logging;
using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.Common.Entities;
using VendingMachine.DAL.Repository.Contracts;

namespace VendingMachine.BLL.Logic
{
    public class DrinkLogic : IDrinkLogic
    {
        private readonly IDrinksRepository _drinksRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ILogger<DrinkLogic> _logger;

        public DrinkLogic(
            IDrinksRepository drinksRepository,
            IBrandRepository brandRepository,
            ILogger<DrinkLogic> logger)
        {
            _drinksRepository = drinksRepository ?? throw new ArgumentNullException(nameof(drinksRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IAsyncEnumerable<Drink> GetAllAsync()
        {
            _logger.LogDebug("GetAllAsync called");
            return _drinksRepository.GetAllAsync();
        }

        public async Task<Drink?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            _logger.LogDebug("GetByIdAsync called with id: {DrinkId}", id);
            return await _drinksRepository.GetDrinkAsync(id);
        }

        public async Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId)
        {
            if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));

            _logger.LogDebug("GetAllByBrandAsync called with brandId: {BrandId}", brandId);

            if (!await _brandRepository.ExistsAsync(brandId))
            {
                _logger.LogWarning("Brand with id {BrandId} does not exist", brandId);
                return Enumerable.Empty<Drink>();
            }

            return await _drinksRepository.GetAllByBrandAsync(brandId);
        }

        public async Task AddAsync(Drink drink)
        {
            if (drink == null) throw new ArgumentNullException(nameof(drink));

            _logger.LogDebug("AddAsync called for drink title: {Title}", drink.Title);

            if (!await _brandRepository.ExistsAsync(drink.BrandId))
            {
                throw new InvalidOperationException($"Brand with id {drink.BrandId} does not exist.");
            }

            await _drinksRepository.CreateDrink(drink);
        }

        public async Task UpdateAsync(Drink drink)
        {
            if (drink == null) throw new ArgumentNullException(nameof(drink));
            if (drink.ItemId <= 0) throw new ArgumentOutOfRangeException(nameof(drink.ItemId));

            _logger.LogDebug("UpdateAsync called for drink id: {DrinkId}", drink.ItemId);

            if (!await _brandRepository.ExistsAsync(drink.BrandId))
            {
                throw new InvalidOperationException($"Brand with id {drink.BrandId} does not exist.");
            }

            var existingDrink = await _drinksRepository.GetDrinkAsync(drink.ItemId);
            if (existingDrink == null)
            {
                throw new KeyNotFoundException($"Drink with id {drink.ItemId} does not exist.");
            }

            await _drinksRepository.EditDrink(drink);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            _logger.LogDebug("DeleteAsync called for drink id: {DrinkId}", id);

            var existingDrink = await _drinksRepository.GetDrinkAsync(id);
            if (existingDrink == null)
            {
                throw new KeyNotFoundException($"Drink with id {id} does not exist.");
            }

            await _drinksRepository.RemoveDrink(existingDrink);
        }

        public async Task BulkUpsertAsync(IEnumerable<Drink> drinks)
        {
            if (drinks == null) throw new ArgumentNullException(nameof(drinks));

            var drinkList = drinks.ToList();
            _logger.LogDebug("BulkUpsertAsync called for {Count} drinks", drinkList.Count);

            if (drinkList.Count == 0)
            {
                return;
            }

            var brandIds = drinkList.Select(d => d.BrandId).Distinct().ToList();
            foreach (var brandId in brandIds)
            {
                if (!await _brandRepository.ExistsAsync(brandId))
                {
                    throw new InvalidOperationException($"Brand with id {brandId} does not exist.");
                }
            }

            await _drinksRepository.BulkUpsertAsync(drinkList);
        }
    }
}
