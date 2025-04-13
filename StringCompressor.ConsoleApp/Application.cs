using StringCompressor.BLL.Logic.Contracts;

namespace StringCompressor.ConsoleApp
{
    internal class Application
    {
        private readonly ICompressionFacade _compression;

        public Application(ICompressionFacade compression)
        {
            _compression = compression;
        }

        // <summary>
        /// Запуск приложения.
        /// </summary>
        public async Task RunAsync()
        {
            Console.WriteLine("String Compression Application");
            Console.WriteLine("-----------------------------");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Compress and save string");
                Console.WriteLine("2. Read and decompress string");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await HandleCompressionAsync();
                        break;
                    case "2":
                        await HandleDecompressionAsync();
                        break;
                    case "3":
                        Console.WriteLine("Exiting application...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private async Task HandleCompressionAsync()
        {
            try
            {
                Console.Write("\nEnter string to compress: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty.");
                    return;
                }

                var compressed = await _compression.CompressAndSaveAsync(input);
                Console.WriteLine($"Compressed successfully. Result: {compressed}");
                Console.WriteLine($"Saved to file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during compression: {ex.Message}");
            }
        }

        private async Task HandleDecompressionAsync()
        {
            try
            {
                var decompressed = await _compression.ReadAndDecompressAsync();
                Console.WriteLine($"\nDecompressed string: {decompressed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during decompression: {ex.Message}");
            }
        }
    }
}
