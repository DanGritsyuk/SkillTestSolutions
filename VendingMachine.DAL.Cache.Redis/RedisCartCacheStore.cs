using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using VendingMachine.Common.Entities;
using VendingMachine.DAL.Cache.Contracts;

namespace VendingMachine.DAL.Cache
{
    public sealed class RedisCartCacheStore : ICartCacheStore
    {
        private const string KeyPrefix = "vm:cart:";
        private const string FieldCartId = "cart_id";
        private const string FieldClientMoney = "client_money";
        private const string FieldItems = "items_json";
        private const string FieldInsertedCoins = "inserted_coins_json";

        private readonly IDatabase _database;
        private readonly ILogger<RedisCartCacheStore> _logger;

        public RedisCartCacheStore(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisCartCacheStore> logger)
        {
            if (connectionMultiplexer == null) throw new ArgumentNullException(nameof(connectionMultiplexer));
            _database = connectionMultiplexer.GetDatabase();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ClientCart?> GetAsync(Guid cartId, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(cartId);

            try
            {
                var values = await _database.HashGetAsync(key, new RedisValue[]
                {
                    FieldCartId,
                    FieldClientMoney,
                    FieldItems,
                    FieldInsertedCoins
                }).ConfigureAwait(false);

                if (values.Length != 4 || values[0].IsNullOrEmpty)
                {
                    _logger.LogDebug("Cart not found in cache: {CartId}", cartId);
                    return null;
                }

                var cart = new ClientCart
                {
                    CartId = Guid.Parse(values[0]!),
                    ClientMoney = decimal.Parse(values[1]!, CultureInfo.InvariantCulture)
                };

                if (!values[2].IsNullOrEmpty)
                {
                    cart.Items = JsonSerializer.Deserialize<List<Drink>>(values[2]!) ?? new List<Drink>();
                }

                if (!values[3].IsNullOrEmpty)
                {
                    cart.InsertedCoins = JsonSerializer.Deserialize<List<Coin>>(values[3]!) ?? new List<Coin>();
                }

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart from cache: {CartId}", cartId);
                throw;
            }
        }

        public async Task SetAsync(ClientCart cart, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            var key = BuildKey(cart.CartId);

            try
            {
                var itemsJson = JsonSerializer.Serialize(cart.Items);
                var insertedCoinsJson = JsonSerializer.Serialize(cart.InsertedCoins);

                var entries = new HashEntry[]
                {
                    new(FieldCartId, cart.CartId.ToString()),
                    new(FieldClientMoney, cart.ClientMoney.ToString(CultureInfo.InvariantCulture)),
                    new(FieldItems, itemsJson),
                    new(FieldInsertedCoins, insertedCoinsJson)
                };

                await _database.HashSetAsync(key, entries).ConfigureAwait(false);

                if (ttl.HasValue)
                {
                    await _database.KeyExpireAsync(key, ttl.Value).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cart in cache: {CartId}", cart.CartId);
                throw;
            }
        }

        public async Task ClearAsync(Guid cartId, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(cartId);

            try
            {
                await _database.KeyDeleteAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cart in cache: {CartId}", cartId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid cartId, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(cartId);

            try
            {
                return await _database.KeyExistsAsync(key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check cart existence in cache: {CartId}", cartId);
                throw;
            }
        }

        private static RedisKey BuildKey(Guid cartId) => KeyPrefix + cartId;
    }
}
