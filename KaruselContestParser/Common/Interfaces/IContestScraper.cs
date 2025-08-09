using KaruselContestParser.Common.Models;

namespace KaruselContestParser.Common.Interfaces
{
    public interface IContestScraper
    {
        Task ScrapeContestAsync();
        Participant FindParticipantByUrl(string workUrl);
        List<Participant> GetTopParticipants(int count);
    }
}