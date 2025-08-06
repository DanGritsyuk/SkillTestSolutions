using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

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
            => await GetResourcesAsync(null, "списка ресурсов");

        public async Task<IEnumerable<Resource>> GetAllActiveResourcesAsync()
            => await GetResourcesAsync(r => r.IsActive, "списка активных ресурсов");

        public async Task<IEnumerable<Resource>> GetAllArchiveResourcesAsync()
            => await GetResourcesAsync(r => !r.IsActive, "списка архивных ресурсов");

        public async Task<Resource> GetResourceByIdAsync(int id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            return resource ?? throw new Exception($"Ресурс с ID {id} не найден");
        }

        public async Task CreateResourceAsync(Resource resource)
        {
            ValidateResource(resource);
            await ValidateResourceNameUniqueness(resource.Name);

            await ExecuteResourceOperationAsync(
                operation: async () =>
                {
                    await _unitOfWork.Resources.AddAsync(resource);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Создан новый ресурс: {resource.Name}",
                errorMessage: "Ошибка при создании ресурса"
            );
        }

        public async Task UpdateResourceAsync(Resource resource)
        {
            ValidateResource(resource);
            var existing = await GetResourceByIdAsync(resource.Id);

            if (existing.Name != resource.Name)
                await ValidateResourceNameUniqueness(resource.Name, resource.Id);

            await ExecuteResourceOperationAsync(
                operation: async () =>
                {
                    existing.Name = resource.Name;
                    existing.IsActive = resource.IsActive;
                    _unitOfWork.Resources.Update(existing);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Обновлён ресурс: {resource.Name} (ID: {resource.Id})",
                errorMessage: "Ошибка при обновлении ресурса"
            );
        }

        public async Task ToggleResourceStatusAsync(int id)
        {
            var resource = await GetResourceByIdAsync(id);

            if (resource.IsActive)
            {
                bool isUsed = await _unitOfWork.Resources.IsUsedInReceiptItemsAsync(id) ||
                              await _unitOfWork.Resources.IsUsedInShipmentItemsAsync(id);

                if (isUsed)
                    throw new Exception("Невозможно архивировать ресурс, так как он используется в документах");
            }

            await ExecuteResourceOperationAsync(
                operation: async () =>
                {
                    resource.IsActive = !resource.IsActive;
                    _unitOfWork.Resources.Update(resource);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Ресурс {resource.Name} {(resource.IsActive ? "восстановлен" : "архивирован")}",
                errorMessage: "Ошибка при изменении статуса ресурса"
            );
        }

        private async Task<IEnumerable<Resource>> GetResourcesAsync(
            Expression<Func<Resource, bool>> predicate,
            string operationName)
        {
            try
            {
                var resources = predicate == null
                    ? await _unitOfWork.Resources.GetAllAsync()
                    : await _unitOfWork.Resources.FindAsync(predicate);

                return resources is IList<Resource> list ? list : resources.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении {operationName}");
                throw new Exception("Ошибка при загрузке данных");
            }
        }

        private void ValidateResource(Resource resource)
        {
            if (string.IsNullOrWhiteSpace(resource.Name))
                throw new Exception("Название ресурса не может быть пустым");
        }

        private async Task ValidateResourceNameUniqueness(string name, int? excludeId = null)
        {
            bool nameExists = excludeId.HasValue
                ? await _unitOfWork.Resources.IsNameUniqueAsync(name, excludeId.Value)
                : await _unitOfWork.Resources.IsNameUniqueAsync(name);

            if (!nameExists)
                throw new Exception("Ресурс с таким именем уже существует");
        }

        private async Task ExecuteResourceOperationAsync(
            Func<Task> operation,
            string successMessage,
            string errorMessage)
        {
            try
            {
                await operation();
                _logger.LogInformation(successMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw new Exception(GetUserFriendlyError(errorMessage));
            }
        }

        private string GetUserFriendlyError(string errorMessage) => errorMessage switch
        {
            _ when errorMessage.Contains("создании") => "Ошибка при сохранении данных",
            _ when errorMessage.Contains("обновлении") => "Ошибка при обновлении данных",
            _ when errorMessage.Contains("статуса") => "Ошибка при изменении статуса",
            _ => "Ошибка операции"
        };
    }
}