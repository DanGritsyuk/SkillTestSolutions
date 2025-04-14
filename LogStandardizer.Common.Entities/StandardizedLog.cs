using LogStandardizer.Common.Entities.InputLogModels;

namespace LogStandardizer.Common.Entities
{
    public class StandardizedLog
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string LogLevel { get; set; }
        public string CallingMethod { get; set; }
        public string Message { get; set; }

        // Метод для преобразования в строку с табуляциями
        public override string ToString()
        {
            return $"{Date}\t{Time}\t{LogLevel}\t{CallingMethod}\t{Message}";
        }
    }
}
