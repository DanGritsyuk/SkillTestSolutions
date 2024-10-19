using System.IO.Compression;
using System.Threading;

namespace ZipFileMetrics
{
    class Program
    {
        private static int maxDepth = 0; // Максимальный уровень вложенности ZIP-архивов.
        private static long length = 0;  // Общий размер данных в байтах.
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        static async Task Main(string[] args)
        {
            // Открываем ZIP-архив для чтения.
            using (var archiveStream = new FileStream("E:\\main.zip", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    await CalcSizeDataAndMaxZipDepthAsync(archiveStream, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine($"Максимальный уровень вложенности: {maxDepth}");
                Console.WriteLine($"Предполагаемый размер данных: {FormatSize(length)}");
            }
        }

        private static async Task CalcSizeDataAndMaxZipDepthAsync(Stream stream, int thisZipDepth)
        {
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                // Перебираем все элементы в архиве.
                await foreach (var item in zip.GetEntriesAsync())
                {
                    int currentDepth = thisZipDepth + GetDepth(item.FullName); // Определяем уровень вложенности текущего элемента.

                    // Если элемент является ZIP-архивом, рекурсивно обрабатываем его.
                    if (item.FullName.EndsWith(".zip"))
                    {

                        try
                        {
                            using (var innerStream = item.Open())
                            {
                                CalcSizeDataAndMaxZipDepthAsync(innerStream, currentDepth + 1); // "+ 1" Добавляем вложеность самого архива
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при обработке архива {item.FullName}: {ex.Message}");
                        }

                    }
                    else
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            // Увеличиваем общий размер данных на размер текущего файла.
                            length += item.Length;

                            if (currentDepth > maxDepth)
                            {

                                if (currentDepth > maxDepth) // двойная проверка
                                {
                                    maxDepth = currentDepth;
                                }
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Форматирует размер в байтах в удобочитаемый вид с единицами измерения.
        /// </summary>
        private static string FormatSize(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Size cannot be negative.");

            string[] sizeUnits = { "байт", "килобайт", "мегабайт", "гигабайт", "терабайт" };
            double size = bytes;
            int unitIndex = 0;

            // Определяем подходящую единицу измерения.
            while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{Math.Round(size, 1)} {sizeUnits[unitIndex]}"; // Возвращаем отформатированный размер.
        }

        private static int GetDepth(string fullName) => fullName.Split('/').Length - 1;
    }

    public static class ZipArchiveExtensions
    {
        public static async IAsyncEnumerable<ZipArchiveEntry> GetEntriesAsync(this ZipArchive zip)
        {
            foreach (var entry in zip.Entries)
            {
                // Симуляция асинхронной операции, если это необходимо
                await Task.Yield(); // Это просто для демонстрации
                yield return entry;
            }
        }
    }
}