namespace LogStandardizer.Common.Entities
{
    public class ProblemLog
    {
        public string OriginalMessage { get; set; }
        public string Reason { get; set; }
        public DateTime DetectedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{DetectedAt:yyyy-MM-dd HH:mm:ss}] {Reason}\n{OriginalMessage}\n";
        }
    }
}
