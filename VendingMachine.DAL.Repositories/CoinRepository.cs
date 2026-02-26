using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendingMachine.Common.Entities;
using VendingMachine.Common.Entities.Enums;
using VendingMachine.DAL.Repository.Contracts;

namespace VendingMachine.DAL.Repository
{
    public class CoinRepository : ICoinRepository
    {
        private readonly VendingMachineDbContext _dbContext;
        private readonly ILogger<CoinRepository> _logger;

        public CoinRepository(VendingMachineDbContext dbContext, ILogger<CoinRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Coin?> GetByIdAsync(int itemId)
        {
            try
            {
                return await _dbContext.Coins.FindAsync(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting coin by ID: {itemId}");
                throw;
            }
        }

        public async Task<Coin?> GetByDenominationAsync(CoinDenomination denomination)
        {
            try
            {
                return await _dbContext.Coins
                    .FirstOrDefaultAsync(c => c.Denomination == denomination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting coin by denomination: {denomination}");
                throw;
            }
        }

        public async Task<IEnumerable<Coin>> GetAllAsync()
        {
            try
            {
                return await _dbContext.Coins.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all coins");
                throw;
            }
        }

        public async Task AddAsync(Coin coin)
        {
            try
            {
                await _dbContext.Coins.AddAsync(coin);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding coin with ID: {coin.ItemId}");
                throw;
            }
        }

        public async Task UpdateAsync(Coin coin)
        {
            try
            {
                _dbContext.Coins.Update(coin);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating coin with ID: {coin.ItemId}");
                throw;
            }
        }

        public async Task DeleteAsync(int itemId)
        {
            try
            {
                var coin = await GetByIdAsync(itemId);
                if (coin != null)
                {
                    _dbContext.Coins.Remove(coin);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting coin with ID: {itemId}");
                throw;
            }
        }
    }
}
