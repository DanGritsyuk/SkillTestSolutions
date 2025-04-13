using StringCompressor.DAL.Repository.Contracts;

namespace StringCompressor.DAL.Repository
{
    public class FileRepository : IFileRepository
    {
        private IFileContext _context;

        public FileRepository(IFileContext context)
        {
            _context = context;
        }

        public async Task<string> GetStringAsync() => await _context.ReadLineAsync();

        public async Task WriteStringAsync(string line) => await _context.WriteLineAsync(line);
    }
}
