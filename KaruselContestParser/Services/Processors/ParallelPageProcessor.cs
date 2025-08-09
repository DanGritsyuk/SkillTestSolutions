using HtmlAgilityPack;
using KaruselContestParser.Common.Interfaces;
using KaruselContestParser.Common.Models;
using KaruselContestParser.Services.Parsers;
using System.Collections.Concurrent;

namespace KaruselContestParser.Services.Processors
{
    public class ParallelPageProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly IPageParser _pageParser;
        private readonly IOutputUI _outputService;
        private readonly ConcurrentBag<Participant> _participants = new ConcurrentBag<Participant>();
        private readonly object _lock = new object();
        private int _currentPage = 1;

        public ParallelPageProcessor(HttpClient httpClient, IPageParser pageParser, IOutputUI outputService)
        {
            _httpClient = httpClient;
            _pageParser = pageParser;
            _outputService = outputService;
        }

        public async Task<List<Participant>> ProcessPagesAsync(int maxDegreeOfParallelism)
        {
            var tasks = new List<Task>();
            using var throttler = new SemaphoreSlim(maxDegreeOfParallelism);

            while (true)
            {
                await throttler.WaitAsync();
                int pageToProcess;

                lock (_lock)
                {
                    if (_currentPage < 1) break;
                    pageToProcess = _currentPage++;
                }

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await ProcessSinglePageAsync(pageToProcess);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return _participants.ToList();
        }

        private async Task ProcessSinglePageAsync(int page)
        {
            try
            {
                _outputService.WritePageProcessing(page);

                var url = page == 1 ?
                    "https://www.karusel-tv.ru/contest/fiksiki2025/photo" :
                    $"https://www.karusel-tv.ru/contest/fiksiki2025/photo?page={page}";

                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                if (_pageParser.HasNoWorksMessage(doc))
                {
                    _outputService.WriteLastPageReached(page - 1);
                    lock (_lock) { _currentPage = -1; } // Stop processing
                    return;
                }

                var participants = _pageParser.ParseParticipants(doc, page).ToList();

                if (!participants.Any())
                {
                    _outputService.WriteNoParticipantsOnPage();
                    lock (_lock) { _currentPage = -1; } // Stop processing
                    return;
                }

                foreach (var participant in participants)
                {
                    _participants.Add(participant);
                }
            }
            catch (Exception ex)
            {
                _outputService.WritePageError(page, ex.Message);
                lock (_lock) { if (_currentPage == page) _currentPage++; } // Skip failed page
            }
        }
    }
}