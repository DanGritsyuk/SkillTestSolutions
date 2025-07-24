using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.Common.Entities;
using VendingMachine.DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace VendingMachine.BLL.Logic
{
    /// <summary>
    /// Business logic for working with drinks.
    /// </summary>
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
            _drinksRepository = drinksRepository;
            _brandRepository = brandRepository;
            _logger = logger;
        }

        /// <summary>
        /// Returns all drinks from the repository.
        /// </summary>
        public IAsyncEnumerable<Drink> GetAllDrinksAsync()
        {
            _logger.LogDebug("Called GetAllDrinksAsync()");
            return _drinksRepository.GetAllAsync();
        }

        /// <summary>
        /// Returns a drink by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the drink.</param>
        /// <returns>Drink entity or null.</returns>
        public async Task<Drink?> GetDrinkByIdAsync(int id)
        {
            _logger.LogDebug($"Called GetDrinkByIdAsync() with id: {id}");

            var drink = await _drinksRepository.GetDrinkAsync(id);

            if (drink == null)
            {
                _logger.LogWarning($"Drink with id {id} not found.");
            }

            return drink;
        }

        /// <summary>
        /// Returns all drinks that belong to a specific brand.
        /// </summary>
        /// <param name="brandId">The ID of the brand.</param>
        /// <returns>List of drinks.</returns>
        public async Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId)
        {
            _logger.LogDebug($"Called GetAllByBrandAsync() with brandId: {brandId}");

            if (!await _brandRepository.ExistsAsync(brandId))
            {
                _logger.LogWarning($"Brand with id {brandId} does not exist.");
                return Enumerable.Empty<Drink>();
            }

            return await _drinksRepository.GetAllByBrandAsync(brandId);
        }
    }

}
