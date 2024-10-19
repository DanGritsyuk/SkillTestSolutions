using System.IO.Compression;

namespace ZipFileMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxDepth = 0; // Максимальный уровень вложенности ZIP-архивов.
            long length = 0;  // Общий размер данных в байтах.

            // Открываем ZIP-архив для чтения.
            using (var archiveStream = new FileStream("E:\\main.zip", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    // Получаем максимальный уровень вложенности и общий размер данных.
                    maxDepth = GetMaxZipDepthAndSizeData(archiveStream, ref length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine($"Максимальный уровень вложенности: {maxDepth}");
                Console.WriteLine($"Предполагаемый размер данных: {FormatSize(length)}");
            }
        }

        /// <summary>
        /// Рекурсивно определяет максимальный уровень вложенности ZIP-архивов и суммирует размеры файлов.
        /// </summary>
        /// <param name="stream">Поток ZIP-архива.</param>
        /// <param name="length">Ссылка на переменную, хранящую общий размер данных.</param>
        /// <returns>Максимальный уровень вложенности архивов.</returns>
        private static int GetMaxZipDepthAndSizeData(Stream stream, ref long length)
        {
            int currentMaxDepth = 0; // Текущий максимальный уровень вложенности.

            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                // Перебираем все элементы в архиве.
                foreach (var item in zip.Entries)
                {
                    int currentDepth = GetDepth(item.FullName); // Определяем уровень вложенности текущего элемента.

                    // Если элемент является ZIP-архивом, рекурсивно обрабатываем его.
                    if (item.FullName.EndsWith(".zip"))
                    {

                        try
                        {
                            using (var innerStream = item.Open())
                            {
                                currentDepth += GetMaxZipDepthAndSizeData(innerStream, ref length) + 1; // "+ 1" Добавляем вложеность самого архива
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при обработке архива {item.FullName}: {ex.Message}");
                        }

                    }
                    else
                    {
                        // Увеличиваем общий размер данных на размер текущего файла.
                        length += item.Length;
                    }

                    if (currentDepth > currentMaxDepth)
                    {
                        currentMaxDepth = currentDepth;
                    }
                }
            }

            return currentMaxDepth; // Возвращаем максимальный уровень вложенности.
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
}