using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.BLL.Logic
{
    public class ResourceLogic : IResourceLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<ResourceLogic> _logger;

        public ResourceLogic(
            INexusUnitOfWork unitOfWork,
            ILogger<ResourceLogic> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                var resources = await _unitOfWork.Resources.GetAllAsync();
                return resources.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка ресурсов");
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        public async Task<IEnumerable<Resource>> GetAllActiveResourcesAsync()
        {
            try
            {
                var resources = await _unitOfWork.Resources.FindAsync(r => r.IsActive);
                return resources;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка ресурсов");
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        public async Task<Resource> GetResourceByIdAsync(int id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            if (resource == null)
            {
                throw new Exception($"Ресурс с ID {id} не найден");
            }
            return resource;
        }

        public async Task CreateResourceAsync(Resource resource)
        {
            ValidateResource(resource);

            if (await _unitOfWork.Resources.IsNameUniqueAsync(resource.Name))
                throw new Exception("Ресурс с таким именем уже существует");

            try
            {
                await _unitOfWork.Resources.AddAsync(resource);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Создан новый ресурс: {resource.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании ресурса");
                throw new Exception("Ошибка при сохранении данных");
            }
        }

        public async Task UpdateResourceAsync(Resource resource)
        {
            ValidateResource(resource);

            var existing = await GetResourceByIdAsync(resource.Id);

            if (existing.Name != resource.Name &&
                await _unitOfWork.Resources.IsNameUniqueAsync(resource.Name, resource.Id))
                throw new Exception("Ресурс с таким именем уже существует");

            try
            {
                existing.Name = resource.Name;
                existing.IsActive = resource.IsActive;

                _unitOfWork.Resources.Update(existing);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Обновлён ресурс: {resource.Name} (ID: {resource.Id})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении ресурса");
                throw new Exception("Ошибка при обновлении данных");
            }
        }

        public async Task ToggleResourceStatusAsync(int id)
        {
            var resource = await GetResourceByIdAsync(id);

            // Проверка использования ресурса в документах при архивации
            if (resource.IsActive &&
                (await _unitOfWork.Resources.IsUsedInReceiptItemsAsync(id) ||
                 await _unitOfWork.Resources.IsUsedInShipmentItemsAsync(id)))
            {
                throw new Exception("Невозможно архивировать ресурс, так как он используется в документах");
            }

            try
            {
                resource.IsActive = !resource.IsActive;
                _unitOfWork.Resources.Update(resource);
                await _unitOfWork.CompleteAsync();

                var action = resource.IsActive ? "восстановлен" : "архивирован";
                _logger.LogInformation($"Ресурс {resource.Name} {action}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении статуса ресурса");
                throw new Exception("Ошибка при изменении статуса");
            }
        }

        private void ValidateResource(Resource resource)
        {
            if (string.IsNullOrWhiteSpace(resource.Name))
                throw new Exception("Название ресурса не может быть пустым");
        }
    }
}
