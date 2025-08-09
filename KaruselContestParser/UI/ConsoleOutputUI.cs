using KaruselContestParser.Common.Interfaces;
using KaruselContestParser.Common.Models;

namespace KaruselContestParser.UI
{
    public class ConsoleOutputUI : IOutputUI
    {
        public void WriteStartProcessing()
            => Console.WriteLine("Начинаем обработку конкурса...");

        public void WritePageProcessing(int page)
            => Console.WriteLine($"Обработка страницы {page}...");

        public void WriteLastPageReached(int lastPage)
            => Console.WriteLine($"Достигнута последняя страница ({lastPage})");

        public void WriteNoParticipantsOnPage()
            => Console.WriteLine("На странице нет участников - завершаем обработку");

        public void WritePageError(int page, string errorMessage)
            => Console.WriteLine($"Ошибка при обработке страницы {page}: {errorMessage}");

        public void WriteCriticalError(string errorMessage)
            => Console.WriteLine($"Критическая ошибка: {errorMessage}");

        public void WriteTopParticipants(IEnumerable<Participant> topParticipants)
        {
            Console.WriteLine("\n🏆 ТОП-10 УЧАСТНИКОВ 🏆");
            foreach (var participant in topParticipants)
            {
                Console.WriteLine($"{participant.GlobalRank} место: {participant.Name} - {participant.Votes} голосов");
                Console.WriteLine($"   Страница: {participant.Page}, Работа: {participant.WorkUrl}\n");
            }
        }

        public void WriteTargetParticipantInfo(Participant targetParticipant, List<Participant> allParticipants)
        {
            if (targetParticipant == null)
            {
                Console.WriteLine("\n⚠️ Целевая работа не найдена!");
                return;
            }

            Console.WriteLine("\n🔍 РЕЗУЛЬТАТ ДЛЯ ВАШЕЙ РАБОТЫ 🔍");
            Console.WriteLine($"Название: {targetParticipant.Name}");
            Console.WriteLine($"Количество голосов: {targetParticipant.Votes}");
            Console.WriteLine($"Позиция на странице: {targetParticipant.PositionOnPage}");
            Console.WriteLine($"Страница размещения: {targetParticipant.Page}");
            Console.WriteLine($"Позиция в рейтинге: {targetParticipant.GlobalRank}/{allParticipants.Count}");
            Console.WriteLine($"Ссылка на работу: {targetParticipant.WorkUrl}");
        }

        public void WriteExitPrompt()
            => Console.WriteLine("\nНажмите любую клавишу для выхода...");

        public void WriteOperationCanceled() 
            => Console.WriteLine("\n🛑 Операция была отменена");
    }
}