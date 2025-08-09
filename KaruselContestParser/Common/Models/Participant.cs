namespace KaruselContestParser.Common.Models
{
    public class Participant
    {
        public string Name { get; set; }
        public int Votes { get; set; }
        public string WorkUrl { get; set; }
        public int Page { get; set; }
        public int PositionOnPage { get; set; }
        public int GlobalRank { get; set; }
        public int SequentialNumber { get; set; }
    }
}
