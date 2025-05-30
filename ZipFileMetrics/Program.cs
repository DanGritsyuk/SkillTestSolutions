using System.IO.Compression;

namespace ZipFileMetrics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var zipProcessor = new ZipProcessor("E:\\main.zip");
            try
            {
                await zipProcessor.ProcessArchiveAsync();
                Console.WriteLine($"Максимальный уровень вложенности: {zipProcessor.MaxDepth}");
                Console.WriteLine($"Предполагаемый размер данных: {SizeFormatter.FormatSize(zipProcessor.TotalSize)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    // Класс для обработки ZIP-файлов
    public class ZipProcessor
    {
        private readonly string _filePath;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public ZipProcessor(string filePath)
        {
            _filePath = filePath;
        }

        public int MaxDepth { get; private set; } = 0;   // Максимальная глубина вложенности
        public long TotalSize { get; private set; } = 0; // Общий размер файлов

        public async Task ProcessArchiveAsync()
        {
            using (var archiveStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                await ProcessZipStreamAsync(archiveStream, 0);
            }
        }

        private async Task ProcessZipStreamAsync(Stream stream, int currentDepth)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                await foreach (var entry in zip.GetEntriesAsync())
                {
                    int entryDepth = currentDepth + GetEntryDepth(entry.FullName); // Вычисляем глубину текущего элемента

                    if (entry.FullName.EndsWith(".zip"))
                    {
                        // Рекурсивно обрабатываем вложенный архив
                        try
                        {
                            using (var innerStream = entry.Open())
                            {
                                ProcessZipStreamAsync(innerStream, entryDepth + 1); // Увеличиваем глубину архива
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при обработке архива {entry.FullName}: {ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            // Увеличиваем общий размер данных и обновляем максимальную глубину
                            TotalSize += entry.Length;
                            if (entryDepth > MaxDepth)
                            {
                                MaxDepth = entryDepth;
                            }
                        }
                    }
                }
            }
        }

        public static int GetEntryDepth(string fullName) => fullName.Split('/').Length - 1;
    }

    // Вспомогательный класс для форматирования размера
    public static class SizeFormatter
    {
        public static string FormatSize(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Размер не может быть отрицательным.");

            string[] sizeUnits = { "байт", "килобайт", "мегабайт", "гигабайт", "терабайт" };
            double size = bytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{Math.Round(size, 1)} {sizeUnits[unitIndex]}";
        }
    }


    // Асинхронное расширение для обработки ZIP-архивов
    public static class ZipArchiveExtensions
    {
        public static async IAsyncEnumerable<ZipArchiveEntry> GetEntriesAsync(this ZipArchive zip)
        {
            foreach (var entry in zip.Entries)
            {
                await Task.Yield(); // Симуляция асинхронной операции
                yield return entry;
            }
        }
    }
}
