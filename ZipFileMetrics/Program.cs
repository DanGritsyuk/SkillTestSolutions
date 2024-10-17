using System.IO.Compression;

namespace ZipFileMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxDepth = 0;
            long length = 0;

            using (var archiveStream = new FileStream("E:\\main.zip", FileMode.Open, FileAccess.Read))
            {
                maxDepth = GetMaxZipDepthAndSizeData(archiveStream, ref length);
            }

            Console.WriteLine($"Максимальный уровень вложенности: {maxDepth}");
            Console.WriteLine($"Предпологаемый размер данных: {FormatSize(length)}");
        }

        private static int GetMaxZipDepthAndSizeData(Stream stream, ref long length)
        {
            int currentMaxDepth = 0;

            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var item in zip.Entries)
                {
                    int currentDepth = GetDepth(item.FullName);

                    if (item.FullName.EndsWith(".zip"))
                    {
                        var innerStream = item.Open();
                        currentDepth += GetMaxZipDepthAndSizeData(innerStream, ref length) + 1;
                    }
                    else
                    {
                        length += item.Length;
                    }

                    if (currentDepth > currentMaxDepth)
                    {
                        currentMaxDepth = currentDepth;
                    }
                }
            }

            return currentMaxDepth;
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Size cannot be negative.");

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

        private static int GetDepth(string fullName) => fullName.Split('/').Length - 1;
    }
}