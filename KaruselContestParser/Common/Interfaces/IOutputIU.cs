using KaruselContestParser.Common.Models;

namespace KaruselContestParser.Common.Interfaces
{
    public interface IOutputUI
    {
        void WriteStartProcessing();
        void WritePageProcessing(int page);
        void WriteLastPageReached(int lastPage);
        void WriteNoParticipantsOnPage();
        void WritePageError(int page, string errorMessage);
        void WriteCriticalError(string errorMessage);
        void WriteTopParticipants(IEnumerable<Participant> topParticipants);
        void WriteTargetParticipantInfo(Participant targetParticipant, List<Participant> allParticipants);
        void WriteExitPrompt();
        void WriteOperationCanceled();
    }
}
