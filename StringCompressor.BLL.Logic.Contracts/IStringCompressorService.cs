namespace StringCompressor.BLL.Logic.Contracts
{
    public interface IStringCompressorService
    {
        /// <summary>
        /// Сжимает заданную строку.
        /// </summary>
        /// <param name="line">Строка, которую необходимо сжать.</param>
        /// <returns>Асинхронная задача, возвращающая сжатую строку.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="line"/> равен null.</exception>
        Task<string> CompressLineAsync(string line);

        /// <summary>
        /// Разжимает заданную строку.
        /// </summary>
        /// <param name="line">Сжатая строка, которую необходимо разжать.</param>
        /// <returns>Асинхронная задача, возвращающая разжатую строку.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="line"/> равен null.</exception>
        Task<string> DecompressLineAsync(string line);
    }
}
