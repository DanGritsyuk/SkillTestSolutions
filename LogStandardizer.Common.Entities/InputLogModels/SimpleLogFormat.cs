namespace LogStandardizer.Common.Entities.InputLogModels
{
    public class SimpleLogFormat : InputLog
    {
        public DateTime LogDate { get; set; }
        public string Level { get; set; }
        public string Content { get; set; }

        public override bool IsValid =>
            !string.IsNullOrWhiteSpace(Level) &&
            !string.IsNullOrWhiteSpace(Content);

        //public override StandardizedLog ToStandardized()
        //{
        //    return new StandardizedLog
        //    {
        //        Date = LogDate.ToString("dd-MM-yyyy"),
        //        Time = LogDate.ToString("HH:mm:ss.fff"),
        //        LogLevel = LogLevelStandardizerService.Standardize(Level),
        //        CallingMethod = "DEFAULT",
        //        Message = Content
        //    };
        //}
    }
}
