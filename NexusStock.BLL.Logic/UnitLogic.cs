using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace NexusStock.BLL.Logic
{
    public class UnitLogic : IUnitLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<UnitLogic> _logger;

        public UnitLogic(
            INexusUnitOfWork unitOfWork,
            ILogger<UnitLogic> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            try
            {
                var units = await _unitOfWork.Units.GetAllAsync();
                return units.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка единиц измерения");
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        public async Task<IEnumerable<Unit>> GetAllActiveUnitsAsync()
        {
            try
            {
                var units = await _unitOfWork.Units.FindAsync(u => u.IsActive);
                return units.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка активных единиц измерения");
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(id);
            if (unit == null)
            {
                throw new Exception($"Единица измерения с ID {id} не найдена");
            }
            return unit;
        }

        public async Task CreateUnitAsync(Unit unit)
        {
            ValidateUnit(unit);

            if (await _unitOfWork.Units.AnyAsync(u => u.Name == unit.Name))
                throw new Exception("Единица измерения с таким названием уже существует");

            try
            {
                await _unitOfWork.Units.AddAsync(unit);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Создана новая единица измерения: {unit.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании единицы измерения");
                throw new Exception("Ошибка при сохранении данных");
            }
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            ValidateUnit(unit);

            var existing = await GetUnitByIdAsync(unit.Id);

            if (existing.Name != unit.Name &&
                await _unitOfWork.Units.AnyAsync(u => u.Name == unit.Name))
                throw new Exception("Единица измерения с таким названием уже существует");

            try
            {
                existing.Name = unit.Name;
                existing.IsActive = unit.IsActive;

                _unitOfWork.Units.Update(existing);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Обновлена единица измерения: {unit.Name} (ID: {unit.Id})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении единицы измерения");
                throw new Exception("Ошибка при обновлении данных");
            }
        }

        public async Task ToggleUnitStatusAsync(int id)
        {
            var unit = await GetUnitByIdAsync(id);

            // Проверка использования единицы измерения в документах
            if (await IsUnitUsedAsync(id))
                throw new Exception("Невозможно архивировать единицу измерения, так как она используется в документах");

            try
            {
                unit.IsActive = !unit.IsActive;
                _unitOfWork.Units.Update(unit);
                await _unitOfWork.CompleteAsync();

                var action = unit.IsActive ? "восстановлена" : "архивирована";
                _logger.LogInformation($"Единица измерения {unit.Name} {action}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении статуса единицы измерения");
                throw new Exception("Ошибка при изменении статуса");
            }
        }

        private void ValidateUnit(Unit unit)
        {
            if (string.IsNullOrWhiteSpace(unit.Name))
                throw new Exception("Название единицы измерения не может быть пустым");
        }

        private async Task<bool> IsUnitUsedAsync(int unitId)
        {
            return await _unitOfWork.StockBalances.AnyAsync(b => b.UnitId == unitId) ||
                   await _unitOfWork.ReceiptItems.AnyAsync(i => i.UnitId == unitId) ||
                   await _unitOfWork.ShipmentItems.AnyAsync(i => i.UnitId == unitId);
        }
    }
}
