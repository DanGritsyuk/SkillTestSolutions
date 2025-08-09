using HtmlAgilityPack;
using KaruselContestParser.Common.Models;

namespace KaruselContestParser.Services.Parsers
{
    public class PageParser : IPageParser
    {
        public bool HasNoWorksMessage(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode(
                "//div[contains(@class, 'alert-info') and contains(., 'Нет отправленных работ')]") != null;
        }

        public IEnumerable<Participant> ParseParticipants(HtmlDocument document, int pageNumber)
        {
            var participants = new List<Participant>();
            var contestItems = document.DocumentNode.SelectNodes("//div[contains(@class, 'krs-contest_item')]");

            if (contestItems == null) yield break;

            for (int i = 0; i < contestItems.Count; i++)
            {
                var item = contestItems[i];
                var nameNode = item.SelectSingleNode(".//a[contains(@class, 'cardBody__name')]/span");
                var votesNode = item.SelectSingleNode(".//span[contains(@class, 'cardImgOverlay__likesCounter')]");
                var workUrlNode = item.SelectSingleNode(".//div[contains(@class, 'cardMini__cardImgTop')]//a");

                if (workUrlNode == null) continue;

                yield return new Participant
                {
                    Name = nameNode?.InnerText.Trim() ?? "Без имени",
                    Votes = votesNode != null ? int.Parse(votesNode.InnerText.Trim()) : 0,
                    WorkUrl = "https://www.karusel-tv.ru" + workUrlNode.GetAttributeValue("href", ""),
                    Page = pageNumber,
                    PositionOnPage = i + 1
                };
            }
        }
    }
}