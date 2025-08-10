using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

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
                var resourceIdsList = resourceIds?.Distinct().ToList();
                var unitIdsList = unitIds?.Distinct().ToList();

                bool noResourceFilter = resourceIdsList == null || !resourceIdsList.Any();
                bool noUnitFilter = unitIdsList == null || !unitIdsList.Any();

                if (noResourceFilter && noUnitFilter)
                {
                    return await _unitOfWork.StockBalances.GetAllWithDetailsAsync();
                }

                Expression<Func<StockBalance, bool>> filter = b =>
                    (noResourceFilter || resourceIdsList!.Contains(b.ResourceId)) &&
                    (noUnitFilter || unitIdsList!.Contains(b.UnitId));

                return await _unitOfWork.StockBalances.GetFilteredWithDetailsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении складских остатков");
                throw;
            }
        }
    }
}
