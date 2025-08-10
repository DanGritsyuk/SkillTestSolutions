using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.BLL.Logic
{
    public class ShipmentLogic : IShipmentLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<ShipmentLogic> _logger;

        public ShipmentLogic(
            INexusUnitOfWork unitOfWork,
            ILogger<ShipmentLogic> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateShipmentAsync(ShipmentDocument shipment)
        {
            ValidateShipment(shipment);

            if (await _unitOfWork.ShipmentDocuments.IsNumberUniqueAsync(shipment.Number))
                throw new Exception("Документ с таким номером уже существует");

            try
            {
                shipment.IsSigned = false;
                await _unitOfWork.ShipmentDocuments.AddAsync(shipment);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Создан документ отгрузки №{shipment.Number}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании документа отгрузки");
                throw new Exception("Ошибка при сохранении документа");
            }
        }

        public async Task UpdateShipmentAsync(ShipmentDocument shipment)
        {
            ValidateShipment(shipment);

            var existing = await GetFullShipmentAsync(shipment.Id);

            if (existing.IsSigned)
                throw new Exception("Нельзя редактировать подписанный документ");

            if (existing.Number != shipment.Number &&
                await _unitOfWork.ShipmentDocuments.IsNumberUniqueAsync(shipment.Number))
                throw new Exception("Документ с таким номером уже существует");

            // Отменяем старое влияние на баланс (если был подписан)
            if (existing.IsSigned)
            {
                await ReverseStockImpactAsync(existing);
            }

            // Обновляем документ
            existing.Number = shipment.Number;
            existing.Date = shipment.Date;
            existing.ClientId = shipment.ClientId;
            existing.Items = shipment.Items;

            // Применяем новое влияние на баланс (если был подписан)
            if (existing.IsSigned)
            {
                await ApplyStockImpactAsync(existing);
            }

            _unitOfWork.ShipmentDocuments.Update(existing);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Документ отгрузки №{shipment.Number} обновлен");
        }

        public async Task DeleteShipmentAsync(int id)
        {
            var shipment = await GetFullShipmentAsync(id);

            if (shipment.IsSigned)
                throw new Exception("Нельзя удалить подписанный документ");

            _unitOfWork.ShipmentDocuments.Remove(shipment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Документ отгрузки №{shipment.Number} удален");
        }

        public async Task<ShipmentDocument> GetShipmentWithItemsAsync(int id)
        {
            return await _unitOfWork.ShipmentDocuments.GetWithItemsAndClientAsync(id)
                ?? throw new Exception($"Документ отгрузки с ID {id} не найден");
        }

        public async Task SignShipmentAsync(int id)
        {
            var shipment = await GetFullShipmentAsync(id);

            if (shipment.IsSigned)
                throw new Exception("Документ уже подписан");

            // Проверка достаточности товаров на складе
            foreach (var item in shipment.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);

                if (balance == null || balance.Quantity < item.Quantity)
                {
                    throw new Exception($"Недостаточно товара: {item.Resource.Name}");
                }
            }

            // Списание со склада
            foreach (var item in shipment.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);
                balance.Quantity -= item.Quantity;
                _unitOfWork.StockBalances.Update(balance);
            }

            shipment.IsSigned = true;
            _unitOfWork.ShipmentDocuments.Update(shipment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Документ отгрузки №{shipment.Number} подписан");
        }

        public async Task RevokeShipmentAsync(int id)
        {
            var shipment = await GetFullShipmentAsync(id);

            if (!shipment.IsSigned)
                throw new Exception("Документ не подписан");

            // Возврат на склад
            foreach (var item in shipment.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);

                if (balance == null)
                {
                    balance = new StockBalance
                    {
                        ResourceId = item.ResourceId,
                        UnitId = item.UnitId,
                        Quantity = 0
                    };
                    await _unitOfWork.StockBalances.AddAsync(balance);
                }

                balance.Quantity += item.Quantity;
                _unitOfWork.StockBalances.Update(balance);
            }

            shipment.IsSigned = false;
            _unitOfWork.ShipmentDocuments.Update(shipment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Документ отгрузки №{shipment.Number} отозван");
        }

        public async Task<IEnumerable<ShipmentDocument>> GetFilteredShipmentsAsync(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            bool? IsSigned,
            IEnumerable<int> clientIds,
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds)
        {
            bool allEmpty = !startDate.HasValue
                && !endDate.HasValue
                && !IsSigned.HasValue
                && (clientIds == null || !clientIds.Any())
                && (resourceIds == null || !resourceIds.Any())
                && (unitIds == null || !unitIds.Any());

            if (allEmpty)
            {
                return await _unitOfWork.ShipmentDocuments.GetFilteredWithDetailsAsync();
            }

            Expression<Func<ShipmentDocument, bool>> filter = sd =>
                (!startDate.HasValue || sd.Date >= startDate.Value) &&
                (!endDate.HasValue || sd.Date <= endDate.Value) &&
                (!IsSigned.HasValue || sd.IsSigned == IsSigned.Value) &&
                (clientIds == null || !clientIds.Any() || clientIds.Contains(sd.ClientId)) &&
                (resourceIds == null || !resourceIds.Any() || sd.Items.Any(i => resourceIds.Contains(i.ResourceId))) &&
                (unitIds == null || !unitIds.Any() || sd.Items.Any(i => unitIds.Contains(i.UnitId)));

            return await _unitOfWork.ShipmentDocuments.GetFilteredWithDetailsAsync(filter);
        }

        private async Task ReverseStockImpactAsync(ShipmentDocument shipment)
        {
            // Возврат товаров на склад (отмена отгрузки)
            foreach (var item in shipment.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);
                if (balance == null)
                {
                    balance = new StockBalance
                    {
                        ResourceId = item.ResourceId,
                        UnitId = item.UnitId,
                        Quantity = 0
                    };
                    await _unitOfWork.StockBalances.AddAsync(balance);
                }
                balance.Quantity += item.Quantity;
                _unitOfWork.StockBalances.Update(balance);
            }
        }

        private async Task ApplyStockImpactAsync(ShipmentDocument shipment)
        {
            // Проверка достаточности и списание
            foreach (var item in shipment.Items)
            {
                var balance = await FindStockBalanceAsync(item.ResourceId, item.UnitId);
                if (balance == null || balance.Quantity < item.Quantity)
                {
                    throw new Exception($"Недостаточно товара: {item.Resource?.Name}");
                }
                balance.Quantity -= item.Quantity;
                _unitOfWork.StockBalances.Update(balance);
            }
        }

        private void ValidateShipment(ShipmentDocument shipment)
        {
            if (string.IsNullOrWhiteSpace(shipment.Number))
                throw new Exception("Номер документа обязателен");

            if (shipment.Items == null || shipment.Items.Count == 0)
                throw new Exception("Документ должен содержать хотя бы одну позицию");

            foreach (var item in shipment.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Количество должно быть положительным числом");
            }
        }

        private async Task<ShipmentDocument> GetFullShipmentAsync(int id)
        {
            return await _unitOfWork.ShipmentDocuments.GetWithItemsAndClientAsync(id)
                ?? throw new Exception($"Документ отгрузки с ID {id} не найден");
        }

        private async Task<StockBalance> FindStockBalanceAsync(int resourceId, int unitId)
        {
            var balances = await _unitOfWork.StockBalances.FindAsync(b =>
                b.ResourceId == resourceId && b.UnitId == unitId);

            return balances.FirstOrDefault();
        }
    }
}
