using Microsoft.Extensions.Logging;
using VendingMachine.BLL.Logic.Contracts;
using VendingMachine.Common.Entities;
using VendingMachine.DAL.Repository.Contracts;

namespace VendingMachine.BLL.Logic
{
    public class BrandLogic : IBrandLogic
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ILogger<BrandLogic> _logger;

        public BrandLogic(IBrandRepository brandRepository, ILogger<BrandLogic> logger)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Brand>> GetAllAsync() =>
            await _brandRepository.GetAllAsync();

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            return await _brandRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Brand brand)
        {
            if (brand == null) throw new ArgumentNullException(nameof(brand));
            if (string.IsNullOrWhiteSpace(brand.Name)) throw new ArgumentException("Brand name is required.", nameof(brand));

            if (await _brandRepository.ExistsAsync(brand.BrandId)) throw new Exception("Brand already exist.");

            await _brandRepository.AddAsync(brand);

            await _brandRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            if (brand == null) throw new ArgumentNullException(nameof(brand));
            if (brand.BrandId <= 0) throw new ArgumentOutOfRangeException(nameof(brand.BrandId));
            if (string.IsNullOrWhiteSpace(brand.Name)) throw new ArgumentException("Brand name is required.", nameof(brand));

            if (!await _brandRepository.ExistsAsync(brand.BrandId)) throw new Exception("Brand not exist.");

            _brandRepository.Update(brand);

            await _brandRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (!await _brandRepository.ExistsAsync(id)) throw new Exception("Brand not exist.");

            await _brandRepository.DeleteAsync(id);

            await _brandRepository.SaveChangesAsync();
        }
    }
}
