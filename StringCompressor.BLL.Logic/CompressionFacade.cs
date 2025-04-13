using System.Threading.Tasks;
using StringCompressor.BLL.Logic.Contracts;
using StringCompressor.DAL.Repository.Contracts;

namespace StringCompressor.BLL.Logic
{
    public class CompressionFacade : ICompressionFacade
    {
        private readonly IStringCompressorService _compressor;
        private readonly IFileRepository _fileRepository;

        public CompressionFacade(
            IStringCompressorService compressor,
            IFileRepository fileRepository)
        {
            _compressor = compressor;
            _fileRepository = fileRepository;
        }

        public async Task<string> CompressAndSaveAsync(string input)
        {
            var compressed = await _compressor.CompressLineAsync(input);
            await _fileRepository.WriteStringAsync(compressed);
            return compressed;
        }

        public async Task<string> ReadAndDecompressAsync()
        {
            var compressed = await _fileRepository.GetStringAsync();
            return await _compressor.DecompressLineAsync(compressed);
        }
    }
}