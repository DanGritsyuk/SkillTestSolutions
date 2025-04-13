using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringCompressor.DAL.Repository.Contracts
{
    public interface IFileContext
    {
        Task<string> ReadLineAsync();
        Task WriteLineAsync(string line);
    }
}
