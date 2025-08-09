using KaruselContestParser.Services.Parsers;
using KaruselContestParser.Services.Scrapers;
using KaruselContestParser.UI;

namespace KaruselContestParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string targetWorkUrl = "https://www.karusel-tv.ru/contest/fiksiki2025/photo/1115";

            // Настройка зависимостей
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var pageParser = new PageParser();
            var outputService = new RichConsoleUI();
            var contestScraper = new ContestScraper(httpClient, pageParser, outputService);

            outputService.WriteStartProcessing();

            try
            {
                await contestScraper.ScrapeContestAsync();
                var topParticipants = contestScraper.GetTopParticipants(10);
                var targetParticipant = contestScraper.FindParticipantByUrl(targetWorkUrl);
                var allParticipants = contestScraper.GetTopParticipants(int.MaxValue);

                outputService.WriteTopParticipants(topParticipants);
                outputService.WriteTargetParticipantInfo(targetParticipant, allParticipants);
            }
            catch (OperationCanceledException)
            {
                outputService.WriteExitPrompt();
            }
            catch (Exception ex)
            {
                outputService.WriteCriticalError(ex.Message);
            }
            finally
            {
                httpClient.Dispose();
            }

            outputService.WriteExitPrompt();
            Console.ReadKey();
        }
    }
}