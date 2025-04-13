namespace StringCompressor.BLL.Logic.Contracts
{
    public interface ICompressionFacade
    {
        Task<string> CompressAndSaveAsync(string input);
        Task<string> ReadAndDecompressAsync();
    }
}