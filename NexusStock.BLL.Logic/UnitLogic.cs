using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

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
            => await GetUnitsAsync(null, "Ошибка при получении списка единиц измерения");

        public async Task<IEnumerable<Unit>> GetAllActiveUnitsAsync()
            => await GetUnitsAsync(u => u.IsActive, "Ошибка при получении списка активных единиц измерения");

        public async Task<IEnumerable<Unit>> GetAllArchiveUnitsAsync()
            => await GetUnitsAsync(u => !u.IsActive, "Ошибка при получении списка архивных единиц измерения");

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(id);
            return unit ?? throw new Exception($"Единица измерения с ID {id} не найдена");
        }

        public async Task CreateUnitAsync(Unit unit)
        {
            ValidateUnit(unit);
            await CheckUnitNameUniqueness(unit.Name);

            await ExecuteUnitOperationAsync(
                operation: async () => {
                    await _unitOfWork.Units.AddAsync(unit);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Создана новая единица измерения: {unit.Name}",
                errorMessage: "Ошибка при создании единицы измерения"
            );
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            ValidateUnit(unit);
            var existing = await GetUnitByIdAsync(unit.Id);

            if (existing.Name != unit.Name)
                await CheckUnitNameUniqueness(unit.Name, existing.Id);

            await ExecuteUnitOperationAsync(
                operation: async () => {
                    existing.Name = unit.Name;
                    existing.IsActive = unit.IsActive;
                    _unitOfWork.Units.Update(existing);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Обновлена единица измерения: {unit.Name} (ID: {unit.Id})",
                errorMessage: "Ошибка при обновлении единицы измерения"
            );
        }

        public async Task ToggleUnitStatusAsync(int id)
        {
            var unit = await GetUnitByIdAsync(id);

            if (!unit.IsActive && await IsUnitUsedAsync(id))
                throw new Exception("Невозможно архивировать единицу измерения, так как она используется в документах");

            await ExecuteUnitOperationAsync(
                operation: async () => {
                    unit.IsActive = !unit.IsActive;
                    _unitOfWork.Units.Update(unit);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Единица измерения {unit.Name} {(unit.IsActive ? "восстановлена" : "архивирована")}",
                errorMessage: "Ошибка при изменении статуса единицы измерения"
            );
        }

        private async Task<IEnumerable<Unit>> GetUnitsAsync(Expression<Func<Unit, bool>> predicate, string errorMessage)
        {
            try
            {
                var units = predicate == null
                    ? await _unitOfWork.Units.GetAllAsync()
                    : await _unitOfWork.Units.FindAsync(predicate);

                return units.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        private void ValidateUnit(Unit unit)
        {
            if (string.IsNullOrWhiteSpace(unit.Name))
                throw new Exception("Название единицы измерения не может быть пустым");
        }

        private async Task CheckUnitNameUniqueness(string name, int? excludeId = null)
        {
            bool nameExists = excludeId.HasValue
                ? await _unitOfWork.Units.AnyAsync(u => u.Name == name && u.Id != excludeId.Value)
                : await _unitOfWork.Units.AnyAsync(u => u.Name == name);

            if (nameExists)
                throw new Exception("Единица измерения с таким названием уже существует");
        }

        private async Task<bool> IsUnitUsedAsync(int unitId)
        {
            return await _unitOfWork.StockBalances.AnyAsync(b => b.UnitId == unitId) ||
                   await _unitOfWork.ReceiptItems.AnyAsync(i => i.UnitId == unitId) ||
                   await _unitOfWork.ShipmentItems.AnyAsync(i => i.UnitId == unitId);
        }

        private async Task ExecuteUnitOperationAsync(Func<Task> operation, string successMessage, string errorMessage)
        {
            try
            {
                await operation();
                _logger.LogInformation(successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw new Exception(GetUserFriendlyErrorMessage(errorMessage));
            }
        }

        private static string GetUserFriendlyErrorMessage(string errorMessage) => errorMessage switch
        {
            _ when errorMessage.Contains("создании") => "Ошибка при сохранении данных",
            _ when errorMessage.Contains("обновлении") => "Ошибка при обновлении данных",
            _ => "Ошибка операции"
        };
    }
}