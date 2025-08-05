using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.BLL.Logic
{
    public class StockLogic : IStockLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<StockLogic> _logger;

        public StockLogic(
            INexusUnitOfWork unitOfWork,
            ILogger<StockLogic> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<StockBalance>> GetFilteredStockBalancesAsync(IEnumerable<int> resourceIds, IEnumerable<int> unitIds)
        {
            try
            {
                // Используем метод с включением связанных данных
                var balances = await _unitOfWork.StockBalances.GetAllWithDetailsAsync();

                // Применяем фильтрацию
                if (resourceIds != null && resourceIds.Any())
                {
                    balances = balances.Where(b => resourceIds.Contains(b.ResourceId));
                }

                if (unitIds != null && unitIds.Any())
                {
                    balances = balances.Where(b => unitIds.Contains(b.UnitId));
                }

                return balances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении складских остатков");
                throw new Exception("Ошибка при загрузке данных");
            }
        }
    }
}
