namespace LogStandardizer.Common.Entities.InputLogModels
{
    public abstract class InputLog
    {
        public string OriginalMessage { get; set; }
        public abstract bool IsValid { get; }
    }
}
