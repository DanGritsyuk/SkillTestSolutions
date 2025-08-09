using HtmlAgilityPack;
using KaruselContestParser.Common.Models;

namespace KaruselContestParser.Services.Parsers
{
    public interface IPageParser
    {
        bool HasNoWorksMessage(HtmlDocument document);
        IEnumerable<Participant> ParseParticipants(HtmlDocument document, int pageNumber);
    }
}
