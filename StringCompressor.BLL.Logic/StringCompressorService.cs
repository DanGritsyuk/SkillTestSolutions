using System.Text;
using StringCompressor.BLL.Logic.Contracts;

namespace StringCompressor.BLL.Logic
{
    public class StringCompressorService : IStringCompressorService
    {
        public string CompressLine(string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (line.Length == 0)
                return string.Empty;

            var result = new StringBuilder();
            ReadOnlySpan<char> span = line.AsSpan();
            int count = 1;

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i] == span[i - 1])
                {
                    count++;
                }
                else
                {
                    result.Append(span[i - 1]);
                    if (count > 1) result.Append(count);
                    count = 1;
                }
            }

            result.Append(span[^1]);
            if (count > 1) result.Append(count);

            return result.ToString();
        }

        public string DecompressLine(string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            if (line.Length == 0)
                return string.Empty;

            ReadOnlySpan<char> span = line.AsSpan();
            var result = new StringBuilder(line.Length * 2);

            int i = 0;
            while (i < span.Length)
            {
                char currentChar = span[i++];

                if (i < span.Length && char.IsDigit(span[i]))
                {
                    int numStart = i;
                    while (i < span.Length && char.IsDigit(span[i]))
                    {
                        i++;
                    }

                    var numberSpan = span.Slice(numStart, i - numStart);
                    if (int.TryParse(numberSpan, out int count) && count > 1)
                    {
                        result.Append(currentChar, count);
                        continue;
                    }
                }

                result.Append(currentChar);
            }

            return result.ToString();
        }

        public async Task<string> CompressLineAsync(string line) => 
            await Task.Run(() => { return CompressLine(line); });

        public async Task<string> DecompressLineAsync(string line) =>
            await Task.Run(() => { return DecompressLine(line); });
    }
}