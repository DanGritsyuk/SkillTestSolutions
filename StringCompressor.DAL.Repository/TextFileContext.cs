using StringCompressor.DAL.Repository.Contracts;

namespace StringCompressor.DAL.Repository
{


    public class TextFileContext : IFileContext
    {
        private string _filePath;

        public TextFileContext(string filePath)
        {
            _filePath = !string.IsNullOrEmpty(filePath) ? filePath :
                throw new InvalidOperationException("Путь к файлу не установлен.");
        }

        /// <summary>
        /// Читает строку из текстового файла.
        /// </summary>
        /// <returns>Считанная строка или null, если достигнут конец файла.</returns>
        public async Task<string> ReadLineAsync()
        {
            using (var streamReader = new StreamReader(_filePath))
                return await streamReader.ReadLineAsync();
        }

        /// <summary>
        /// Записывает строку в текстовый файл.
        /// </summary>
        /// <param name="line">Строка для записи.</param>
        public async Task WriteLineAsync(string line)
        {
            using (var streamWriter = new StreamWriter(_filePath, append: true))
                await streamWriter.WriteLineAsync(line);
        }
    }

}
