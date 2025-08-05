using Microsoft.Extensions.Logging;
using NexusStock.BLL.Logic.Contracts;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

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
        {
            _logger.LogInformation("Запрос всех клиентов");
            try
            {
                var result = await _unitOfWork.Clients.GetAllAsync();
                var list = result.ToList();
                _logger.LogInformation("Найдено {Count} клиентов", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех клиентов");
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllActiveClientsAsync()
        {
            _logger.LogInformation("Запрос всех активных клиентов");
            try
            {
                var result = await _unitOfWork.Clients.FindAsync(c => c.IsActive);
                var list = result.ToList();
                _logger.LogInformation("Найдено {Count} активных клиентов", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных клиентов");
                throw;
            }
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            _logger.LogInformation("Запрос клиента по Id={ClientId}", id);
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null)
                {
                    _logger.LogWarning("Клиент с Id={ClientId} не найден", id);
                    throw new InvalidOperationException("Клиент не найден");
                }
                return client;
            }
            catch (InvalidOperationException)
            {
                throw; // уже залоггировано как warning
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента Id={ClientId}", id);
                throw;
            }
        }

        public async Task CreateClientAsync(Client client)
        {
            _logger.LogInformation("Создание нового клиента с именем '{ClientName}'", client?.Name);
            if (client == null) throw new ArgumentNullException(nameof(client));
            try
            {
                if (await _unitOfWork.Clients.AnyAsync(c => c.Name == client.Name))
                {
                    _logger.LogWarning("Клиент с именем '{ClientName}' уже существует", client.Name);
                    throw new InvalidOperationException("Клиент с таким именем уже существует");
                }

                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Клиент '{ClientName}' успешно создан (Id={ClientId})",
                                        client.Name, client.Id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании клиента '{ClientName}'", client.Name);
                throw;
            }
        }

        public async Task UpdateClientAsync(Client client)
        {
            _logger.LogInformation("Обновление клиента Id={ClientId}", client?.Id);
            if (client == null) throw new ArgumentNullException(nameof(client));
            try
            {
                var existing = await GetClientByIdAsync(client.Id);

                if (existing.Name != client.Name &&
                    await _unitOfWork.Clients.AnyAsync(c => c.Name == client.Name))
                {
                    _logger.LogWarning("Попытка переименовать клиента Id={ClientId} в существующее имя '{NewName}'",
                                       client.Id, client.Name);
                    throw new InvalidOperationException("Клиент с таким именем уже существует");
                }

                existing.Name = client.Name;
                existing.Address = client.Address;

                _unitOfWork.Clients.Update(existing);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Клиент Id={ClientId} успешно обновлён", client.Id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении клиента Id={ClientId}", client.Id);
                throw;
            }
        }

        public async Task ToggleClientStatusAsync(int id)
        {
            _logger.LogInformation("Переключение статуса клиента Id={ClientId}", id);
            try
            {
                var client = await GetClientByIdAsync(id);
                client.IsActive = !client.IsActive;

                _unitOfWork.Clients.Update(client);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Статус клиента Id={ClientId} теперь IsActive={IsActive}",
                                        id, client.IsActive);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при переключении статуса клиента Id={ClientId}", id);
                throw;
            }
        }
    }
}
