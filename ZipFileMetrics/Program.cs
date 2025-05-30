using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ZipAnalyzerApp
{
    public interface IArchiveAnalyzer
    {
        ArchiveAnalysisResult Analyze(string archivePath);
    }

    public class ArchiveAnalysisResult
    {
        public int MaxDepth { get; set; }
        public long TotalUncompressedSize { get; set; }
        public int ProcessedArchives { get; set; }
        public int ProcessedFiles { get; set; }
    }

    public class ZipAnalyzer : IArchiveAnalyzer
    {
        private readonly int _maxDepthLimit;
        private readonly int _maxArchiveSizeMb;

        public ZipAnalyzer(int maxDepthLimit = 10, int maxArchiveSizeMb = 100)
        {
            _maxDepthLimit = maxDepthLimit;
            _maxArchiveSizeMb = maxArchiveSizeMb;
        }

        public ArchiveAnalysisResult Analyze(string archivePath)
        {
            var result = new ArchiveAnalysisResult();
            var stack = new Stack<ArchiveContext>();

            // Начинаем с корневого архива
            stack.Push(new ArchiveContext(archivePath, 0));

            while (stack.Count > 0)
            {
                var context = stack.Pop();

                try
                {
                    using var fileStream = new FileStream(context.Path, FileMode.Open, FileAccess.Read);
                    using var zip = new ZipArchive(fileStream, ZipArchiveMode.Read);

                    foreach (var entry in zip.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                            continue; // Пропускаем директории

                        result.ProcessedFiles++;
                        result.TotalUncompressedSize += entry.Length;

                        // Обновляем максимальную глубину
                        var currentDepth = context.Depth + GetPathDepth(entry.FullName);
                        result.MaxDepth = Math.Max(result.MaxDepth, currentDepth);

                        // Если это вложенный архив и не превышен лимит глубины
                        if (IsZipArchive(entry.Name) && currentDepth < _maxDepthLimit)
                        {
                            var nestedArchivePath = ExtractNestedArchive(entry);
                            stack.Push(new ArchiveContext(nestedArchivePath, currentDepth + 1));
                            result.ProcessedArchives++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке {context.Path}: {ex.Message}");
                    // Продолжаем обработку других архивов
                }
                finally
                {
                    // Удаляем временные файлы для вложенных архивов
                    if (context.IsTempFile && File.Exists(context.Path))
                    {
                        File.Delete(context.Path);
                    }
                }
            }

            return result;
        }

        private string ExtractNestedArchive(ZipArchiveEntry entry)
        {
            // Проверка размера перед извлечением
            if (entry.Length > _maxArchiveSizeMb * 1024 * 1024)
            {
                throw new InvalidOperationException(
                    $"Размер вложенного архива ({entry.Length} байт) превышает максимально допустимый ({_maxArchiveSizeMb} MB)");
            }

            var tempPath = Path.GetTempFileName();
            using (var entryStream = entry.Open())
            using (var tempFile = File.Create(tempPath))
            {
                entryStream.CopyTo(tempFile);
            }

            return tempPath;
        }

        private static bool IsZipArchive(string fileName)
        {
            return fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetPathDepth(string path)
        {
            return path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Length - 1;
        }

        private class ArchiveContext
        {
            public string Path { get; }
            public int Depth { get; }
            public bool IsTempFile { get; }

            public ArchiveContext(string path, int depth, bool isTempFile = false)
            {
                Path = path;
                Depth = depth;
                IsTempFile = isTempFile;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const string zipPath = "E:\\main.zip";

            try
            {
                var analyzer = new ZipAnalyzer(maxDepthLimit: 10, maxArchiveSizeMb: 50);
                var result = analyzer.Analyze(zipPath);

                Console.WriteLine("Результаты анализа:");
                Console.WriteLine($"- Максимальный уровень вложенности: {result.MaxDepth}");
                Console.WriteLine($"- Общий размер данных: {SizeFormatter.FormatSize(result.TotalUncompressedSize)}");
                Console.WriteLine($"- Обработано архивов: {result.ProcessedArchives}");
                Console.WriteLine($"- Обработано файлов: {result.ProcessedFiles}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }
        }
    }

    public static class SizeFormatter
    {
        public static string FormatSize(long bytes)
        {
            string[] sizeUnits = { "байт", "КБ", "МБ", "ГБ", "ТБ" };
            double size = bytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:0.##} {sizeUnits[unitIndex]}";
        }
    }
}