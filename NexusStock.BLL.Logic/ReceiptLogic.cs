using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.BLL.Logic
{
    public class ReceiptLogic : IReceiptLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<ReceiptLogic> _logger;

        public ReceiptLogic(
            INexusUnitOfWork unitOfWork,
            ILogger<ReceiptLogic> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateReceiptAsync(ReceiptDocument document)
        {
            ValidateReceipt(document);

            if (await _unitOfWork.ReceiptDocuments.IsNumberUniqueAsync(document.Number))
                throw new Exception("Документ с таким номером уже существует");

            try
            {
                await _unitOfWork.ReceiptDocuments.AddAsync(document);
                await UpdateStockBalance(document, isRevert: false);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Создан документ поступления №{document.Number}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании документа поступления");
                throw new Exception("Ошибка при сохранении документа");
            }
        }

        public async Task UpdateReceiptAsync(ReceiptDocument document)
        {
            ValidateReceipt(document);

            var existing = await GetFullReceiptAsync(document.Id);

            if (existing.Number != document.Number &&
                await _unitOfWork.ReceiptDocuments.IsNumberUniqueAsync(document.Number))
                throw new Exception("Документ с таким номером уже существует");

            // Отменить старое влияние на баланс
            await UpdateStockBalance(existing, isRevert: true);

            // Обновить документ
            existing.Number = document.Number;
            existing.Date = document.Date;
            existing.Items = document.Items;

            // Применить новое влияние на баланс
            await UpdateStockBalance(existing, isRevert: false);

            _unitOfWork.ReceiptDocuments.Update(existing);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Обновлен документ поступления №{document.Number}");
        }

        public async Task DeleteReceiptAsync(int id)
        {
            var document = await GetFullReceiptAsync(id);
            await UpdateStockBalance(document, isRevert: true);
            _unitOfWork.ReceiptDocuments.Remove(document);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Удален документ поступления №{document.Number}");
        }

        public async Task<ReceiptDocument> GetReceiptWithItemsAsync(int id)
        {
            return await _unitOfWork.ReceiptDocuments.GetWithItemsAsync(id)
                ?? throw new Exception($"Документ поступления с ID {id} не найден");
        }

        public async Task<IEnumerable<ReceiptDocument>> GetFilteredReceiptsAsync(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            IEnumerable<int> documentIds,
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds)
        {
            bool allEmpty = !startDate.HasValue
                && !endDate.HasValue
                && (documentIds == null || !documentIds.Any())
                && (resourceIds == null || !resourceIds.Any())
                && (unitIds == null || !unitIds.Any());

            if (allEmpty)
            {
                return await _unitOfWork.ReceiptDocuments.GetFilteredWithDetailsAsync();
            }

            Expression<Func<ReceiptDocument, bool>> filter = rd =>
                (!startDate.HasValue || rd.Date >= startDate.Value) &&
                (!endDate.HasValue || rd.Date <= endDate.Value) &&
                (documentIds == null || !documentIds.Any() || documentIds.Contains(rd.Id)) &&
                (resourceIds == null || !resourceIds.Any() || rd.Items.Any(i => resourceIds.Contains(i.ResourceId))) &&
                (unitIds == null || !unitIds.Any() || rd.Items.Any(i => unitIds.Contains(i.UnitId)));

            return await _unitOfWork.ReceiptDocuments.GetFilteredWithDetailsAsync(filter); 
        }

        private async Task UpdateStockBalance(ReceiptDocument document, bool isRevert)
        {
            var multiplier = isRevert ? -1 : 1;

            foreach (var item in document.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);

                if (balance != null)
                {
                    balance.Quantity += item.Quantity * multiplier;
                    _unitOfWork.StockBalances.Update(balance);
                }
                else
                {
                    await _unitOfWork.StockBalances.AddAsync(new StockBalance
                    {
                        ResourceId = item.ResourceId,
                        UnitId = item.UnitId,
                        Quantity = item.Quantity * multiplier
                    });
                }
            }
        }

        private void ValidateReceipt(ReceiptDocument document)
        {
            if (string.IsNullOrWhiteSpace(document.Number))
                throw new Exception("Номер документа обязателен");

            if (document.Items == null || !document.Items.Any())
                throw new Exception("Документ должен содержать хотя бы одну позицию");

            foreach (var item in document.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Количество должно быть положительным числом");
            }
        }

        private async Task<ReceiptDocument> GetFullReceiptAsync(int id)
        {
            return await _unitOfWork.ReceiptDocuments.GetWithItemsAsync(id)
                ?? throw new Exception($"Документ поступления с ID {id} не найден");
        }

        private async Task<StockBalance> FindStockBalanceAsync(int resourceId, int unitId)
        {
            var balances = await _unitOfWork.StockBalances.FindAsync(b =>
                b.ResourceId == resourceId && b.UnitId == unitId);

            return balances.FirstOrDefault();
        }
    }
}
