using KaruselContestParser.Common.Interfaces;
using KaruselContestParser.Common.Models;
using KaruselContestParser.Services.Parsers;
using KaruselContestParser.Services.Processors;

namespace KaruselContestParser.Services.Scrapers
{
    public class ContestScraper : IContestScraper
    {
        private readonly HttpClient _httpClient;
        private readonly IPageParser _pageParser;
        private readonly IOutputUI _outputService;
        private List<Participant> _participants = new List<Participant>();

        public ContestScraper(HttpClient httpClient, IPageParser pageParser, IOutputUI outputService)
        {
            _httpClient = httpClient;
            _pageParser = pageParser;
            _outputService = outputService;
        }

        public async Task ScrapeContestAsync()
        {
            var processor = new ParallelPageProcessor(
                _httpClient,
                _pageParser,
                _outputService
            );

            _participants = await processor.ProcessPagesAsync(maxDegreeOfParallelism: 3);
            UpdateGlobalRanks();
        }

        private void UpdateGlobalRanks()
        {
            var rankedParticipants = _participants
                .OrderByDescending(p => p.Votes)
                .ToList();

            for (int i = 0; i < rankedParticipants.Count; i++)
            {
                rankedParticipants[i].GlobalRank = i + 1;
            }
        }

        public Participant FindParticipantByUrl(string workUrl)
        {
            return _participants.FirstOrDefault(p =>
                p.WorkUrl.Equals(workUrl, StringComparison.OrdinalIgnoreCase));
        }

        public List<Participant> GetTopParticipants(int count)
        {
            return _participants
                .OrderByDescending(p => p.Votes)
                .Take(count)
                .ToList();
        }
    }
}