namespace StringCompressor.DAL.Repository.Contracts
{
    public interface IFileRepository
    {
        Task<string> GetStringAsync();
        Task WriteStringAsync(string line);
    }
}
