namespace LogStandardizer.Common.Entities.InputLogModels
{
    public class DetailedLogFormat : InputLog
    {
        public DateTime LogDate { get; set; }
        public string TimePart { get; set; } // Если время хранится отдельно
        public string Level { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }

        public override bool IsValid =>
            !string.IsNullOrWhiteSpace(Level) &&
            !string.IsNullOrWhiteSpace(Message) &&
            !string.IsNullOrWhiteSpace(Method);
    }
}
