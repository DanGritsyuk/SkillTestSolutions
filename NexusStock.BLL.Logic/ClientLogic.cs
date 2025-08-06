using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.BLL.Logic
{
    public class ClientLogic : IClientLogic
    {
        private readonly INexusUnitOfWork _unitOfWork;
        private readonly ILogger<ClientLogic> _logger;

        public ClientLogic(INexusUnitOfWork unitOfWork, ILogger<ClientLogic> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
            => await GetClientsAsync(null, "всех клиентов", "Найдено {Count} клиентов");

        public async Task<IEnumerable<Client>> GetAllActiveClientsAsync()
            => await GetClientsAsync(c => c.IsActive, "активных клиентов", "Найдено {Count} активных клиентов");

        public async Task<IEnumerable<Client>> GetAllArchiveClientsAsync()
            => await GetClientsAsync(c => !c.IsActive, "архивных клиентов", "Найдено {Count} архивных клиентов");

        public async Task<Client> GetClientByIdAsync(int id)
        {
            _logger.LogInformation("Запрос клиента по Id={ClientId}", id);
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                return client ?? throw new InvalidOperationException("Клиент не найден");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Клиент с Id={ClientId} не найден", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента Id={ClientId}", id);
                throw;
            }
        }

        public async Task CreateClientAsync(Client client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            _logger.LogInformation("Создание нового клиента с именем '{ClientName}'", client.Name);

            await ValidateClientNameUniqueness(client.Name);
            await ExecuteClientOperationAsync(
                operation: async () => {
                    await _unitOfWork.Clients.AddAsync(client);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Клиент '{client.Name}' успешно создан (Id={client.Id})",
                errorMessage: $"Ошибка при создании клиента '{client.Name}'",
                clientName: client.Name
            );
        }

        public async Task UpdateClientAsync(Client client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            _logger.LogInformation("Обновление клиента Id={ClientId}", client.Id);

            var existing = await GetClientByIdAsync(client.Id);

            if (existing.Name != client.Name)
                await ValidateClientNameUniqueness(client.Name, existing.Id);

            await ExecuteClientOperationAsync(
                operation: async () => {
                    existing.Name = client.Name;
                    existing.Address = client.Address;
                    _unitOfWork.Clients.Update(existing);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Клиент Id={client.Id} успешно обновлён",
                errorMessage: $"Ошибка при обновлении клиента Id={client.Id}",
                clientName: client.Name
            );
        }

        public async Task ToggleClientStatusAsync(int id)
        {
            _logger.LogInformation("Переключение статуса клиента Id={ClientId}", id);
            var client = await GetClientByIdAsync(id);

            await ExecuteClientOperationAsync(
                operation: async () => {
                    client.IsActive = !client.IsActive;
                    _unitOfWork.Clients.Update(client);
                    await _unitOfWork.CompleteAsync();
                },
                successMessage: $"Статус клиента Id={id} теперь IsActive={client.IsActive}",
                errorMessage: $"Ошибка при переключении статуса клиента Id={id}",
                clientName: client.Name
            );
        }

        private async Task<IEnumerable<Client>> GetClientsAsync(
            Expression<Func<Client, bool>> predicate,
            string operationName,
            string successMessage)
        {
            _logger.LogInformation("Запрос {OperationName}", operationName);
            try
            {
                var result = predicate == null
                    ? await _unitOfWork.Clients.GetAllAsync()
                    : await _unitOfWork.Clients.FindAsync(predicate);

                var list = result.ToList();
                _logger.LogInformation(successMessage, list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении {OperationName}", operationName);
                throw;
            }
        }

        private async Task ValidateClientNameUniqueness(string name, int? excludeId = null)
        {
            bool nameExists = excludeId.HasValue
                ? await _unitOfWork.Clients.AnyAsync(c => c.Name == name && c.Id != excludeId.Value)
                : await _unitOfWork.Clients.AnyAsync(c => c.Name == name);

            if (nameExists)
            {
                var message = $"Клиент с именем '{name}' уже существует";
                _logger.LogWarning(message);
                throw new InvalidOperationException(message);
            }
        }

        private async Task ExecuteClientOperationAsync(
            Func<Task> operation,
            string successMessage,
            string errorMessage,
            string clientName = null)
        {
            try
            {
                await operation();
                _logger.LogInformation(successMessage);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }
    }
}