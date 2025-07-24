using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendingMachine.Common.Entities;
using VendingMachine.DAL.Repository.Contracts;

namespace VendingMachine.DAL.Repository
{
    public class DrinksRepository : IDrinksRepository
    {
        private readonly VendingMachineDbContext _dbContext;
        private readonly ILogger<DrinksRepository> _logger;

        public DrinksRepository(VendingMachineDbContext dbContext, ILogger<DrinksRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Drink> GetDrinkAsync(int id)
        {
            try
            {
                var drink = await _dbContext.Drinks.FirstAsync(dr => dr.ItemId == id);
                _logger.LogDebug($"Successfully retrieved drink with ID: {id}");
                return drink;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Drink with ID {id} not found");
                throw new KeyNotFoundException($"Drink with ID {id} not found", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting drink with ID: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId)
        {
            _logger.LogInformation("Fetching all drinks for brand ID: {BrandId}", brandId);

            try
            {
                var drinks = await _dbContext.Drinks
                    .Where(d => d.BrandId == brandId)
                    .ToListAsync();

                _logger.LogDebug("Retrieved {Count} drinks for brand ID: {BrandId}", drinks.Count, brandId);

                return drinks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching drinks for brand ID: {BrandId}", brandId);
                throw;
            }
        }

        public IAsyncEnumerable<Drink> GetAllAsync()
        {
            try
            {
                var drinks = _dbContext.Drinks.AsAsyncEnumerable();
                _logger.LogDebug($"");
                return drinks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all drinks");
                throw;
            }
        }

        public async Task CreateDrink(Drink drink)
        {
            _logger.LogInformation($"Creating new drink with ID: {drink.ItemId}");

            try
            {
                _dbContext.Add(drink);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully created drink with ID: {drink.ItemId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating drink with ID: {drink.ItemId}");
                throw;
            }
        }

        public async Task EditDrink(Drink drink)
        {
            _logger.LogInformation($"Updating drink with ID: {drink.ItemId}");

            try
            {
                _dbContext.Update(drink);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated drink with ID: {drink.ItemId}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency conflict while updating drink with ID: {drink.ItemId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating drink with ID: {drink.ItemId}");
                throw;
            }
        }

        public async Task RemoveDrink(Drink drink)
        {
            _logger.LogInformation($"Removing drink with ID: {drink.ItemId}");

            try
            {
                _dbContext.Drinks.Remove(drink);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully removed drink with ID: {drink.ItemId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while removing drink with ID: {drink.ItemId}");
                throw;
            }
        }

        public async Task InsertOrUpdateRangeAsync(IEnumerable<Drink> drinks)
        {
            var drinkList = drinks.ToList();
            _logger.LogInformation($"Bulk inserting/updating {drinkList.Count} drinks");

            try
            {
                var existingIds = drinkList.Where(d => d.ItemId != 0).Select(d => d.ItemId).ToHashSet();
                var existingDrinks = await _dbContext.Drinks
                    .Where(d => existingIds.Contains(d.ItemId))
                    .ToDictionaryAsync(d => d.ItemId);

                foreach (var drink in drinkList)
                {
                    if (existingDrinks.TryGetValue(drink.ItemId, out var existing))
                    {
                        _logger.LogDebug($"Updating existing drink with ID: {drink.ItemId}");
                        _dbContext.Entry(existing).CurrentValues.SetValues(drink);
                    }
                    else
                    {
                        _logger.LogDebug($"Adding new drink with ID: {drink.ItemId}");
                        await _dbContext.Drinks.AddAsync(drink);
                    }
                }

                var savedCount = await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully processed {savedCount} drinks (inserted/updated)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during bulk insert/update of drinks");
                throw;
            }
        }
    }
}
