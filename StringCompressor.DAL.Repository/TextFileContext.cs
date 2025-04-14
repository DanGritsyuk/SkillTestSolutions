using StringCompressor.DAL.Repository.Contracts;

namespace StringCompressor.DAL.Repository
{


    public class TextFileContext : IFileContext
    {
        private string _filePath;

        public TextFileContext(string filePath)
        {
            _filePath = !string.IsNullOrEmpty(filePath) ? filePath :
                throw new InvalidOperationException("The file path is not set.");
        }


        public async Task<string> ReadLineAsync()
        {
            using (var streamReader = new StreamReader(_filePath))
                return await streamReader.ReadLineAsync();
        }

        public async Task WriteLineAsync(string line)
        {
            using (var streamWriter = new StreamWriter(_filePath, append: true))
                await streamWriter.WriteLineAsync(line);
        }
    }

}
