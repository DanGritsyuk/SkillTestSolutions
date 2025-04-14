using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringCompressor.DAL.Repository.Contracts
{
    public interface IFileContext
    {
        /// <summary>
        /// Читает строку из текстового файла.
        /// </summary>
        /// <returns>Считанная строка или null, если достигнут конец файла.</returns>
        Task<string> ReadLineAsync();

        /// <summary>
        /// Записывает строку в текстовый файл.
        /// </summary>
        /// <param name="line">Строка для записи.</param>
        Task WriteLineAsync(string line);
    }
}
