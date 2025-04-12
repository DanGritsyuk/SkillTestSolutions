using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;

namespace ZipFileMetrics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var zipProcessor = new ZipProcessor("E:\\main.zip");
            try
            {
                await zipProcessor.ProcessArchiveAsync();
                Console.WriteLine($"Максимальный уровень вложенности: {zipProcessor.MaxNestingLevel}");
                Console.WriteLine($"Предполагаемый размер данных: {SizeFormatter.FormatSize(zipProcessor.TotalFileSize)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }

    public class ZipProcessor
    {
        private readonly string _archivePath;

        private ConcurrentQueue<Task> _processingTasks;
        private ConcurrentQueue<long> _fileSizesQueue;
        private ConcurrentQueue<int> _nestingLevelsQueue;

        private bool _isProcessingZip;
        private bool _isCalculating;

        public ZipProcessor(string archivePath)
        {
            _archivePath = archivePath;
            _processingTasks = new();
            _fileSizesQueue = new();
            _nestingLevelsQueue = new();
            _isProcessingZip = false;
            _isCalculating = false;
        }

        public int MaxNestingLevel { get; private set; } = 0;   // Максимальный уровень вложенности архивов
        public long TotalFileSize { get; private set; } = 0;    // Общий размер всех файлов

        public async Task ProcessArchiveAsync()
        {
            using (var archiveStream = new FileStream(_archivePath, FileMode.Open, FileAccess.Read))
            {
                Task? calculationTask = null;

                _processingTasks.Enqueue(ProcessZipStreamAsync(archiveStream, 0));
                _isProcessingZip = true;

                calculationTask = Task.Run(CalculateMetrics);
                _isCalculating = true;

                await AwaitAllZipProcesses();
                _isCalculating = false;
                await calculationTask;
            }
        }

        private async Task ProcessZipStreamAsync(Stream stream, int currentNestingLevel)
        {
            int maxDepthInCurrentZip = 0;

            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                await foreach (var entry in zip.GetEntriesAsync())
                {
                    int entryNestingLevel = currentNestingLevel + GetEntryDepth(entry.FullName);

                    if (entry.FullName.EndsWith(".zip"))
                    {
                        try
                        {
                            using (var innerStream = entry.Open())
                            {
                                _processingTasks.Enqueue(ProcessZipStreamAsync(innerStream, entryNestingLevel + 1));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при обработке архива {entry.FullName}: {ex.Message}");
                        }
                    }
                    else
                    {
                        _fileSizesQueue.Enqueue(entry.Length);
                        if (entryNestingLevel > maxDepthInCurrentZip)
                        {
                            maxDepthInCurrentZip = entryNestingLevel;
                        }
                    }
                }
            }
            _nestingLevelsQueue.Enqueue(maxDepthInCurrentZip);
        }

        private void CalculateMetrics()
        {
            while (_isCalculating || (_fileSizesQueue.Count > 0 && _nestingLevelsQueue.Count > 0))
            {
                if (_fileSizesQueue.TryDequeue(out long fileSize))
                    TotalFileSize += fileSize;

                if (_nestingLevelsQueue.TryDequeue(out int nestingLevel))
                    MaxNestingLevel = Math.Max(MaxNestingLevel, nestingLevel);
            }
        }

        private async Task AwaitAllZipProcesses()
        {
            while (_isProcessingZip || _processingTasks.Count > 0)
            {
                if (_processingTasks.TryDequeue(out Task? task))
                    await task!;
            }
        }

        private static int GetEntryDepth(string fullName) => fullName.Split('/').Length - 1;
    }

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

    public static class ZipArchiveExtensions
    {
        public static async IAsyncEnumerable<ZipArchiveEntry> GetEntriesAsync(this ZipArchive zip)
        {
            foreach (var entry in zip.Entries)
            {
                await Task.Yield();
                yield return entry;
            }
        }
    }
}