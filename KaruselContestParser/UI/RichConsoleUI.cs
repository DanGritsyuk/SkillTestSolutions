using KaruselContestParser.Common.Interfaces;
using KaruselContestParser.Common.Models;
using System.Text;

namespace KaruselContestParser.UI
{
    public class RichConsoleUI : IOutputUI
    {
        private int _currentProgress;
        private readonly Queue<(int page, string status)> _recentPageStatuses = new Queue<(int, string)>();
        private const int MaxRecentStatuses = 10;
        private bool _isComplete;
        private int _progressAreaTop = -1;

        public RichConsoleUI()
        {
            Console.OutputEncoding = Encoding.UTF8;
            _isComplete = false;
        }

        public void WriteStartProcessing()
        {
            Console.Clear();
            Console.WriteLine("🚀 Запуск обработки конкурса...");
            Console.WriteLine();
            _progressAreaTop = Console.CursorTop;
        }

        public void WritePageProcessing(int page)
        {
            _currentProgress = page;
            EnqueueStatus(page, "✅");
            UpdateProgressBar();
        }

        public void WriteLastPageReached(int lastPage)
        {
            if (_isComplete) return;

            EnqueueStatus(lastPage, "✅");
            _currentProgress = lastPage;
            UpdateProgressBar();

            // Очищаем область прогресса и выводим итог
            int currentCursorTop = Console.CursorTop;
            int currentCursorLeft = Console.CursorLeft;

            Console.SetCursorPosition(0, _progressAreaTop);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, _progressAreaTop);
            Console.WriteLine($"✨ Обработка завершена! Всего обработано страниц: {lastPage}");

            // Устанавливаем курсор для последующего вывода
            Console.SetCursorPosition(0, _progressAreaTop + 1);
            _isComplete = true;
        }

        public void WriteNoParticipantsOnPage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nℹ️ На странице нет участников - завершение обработки");
            Console.ResetColor();
        }

        public void WritePageError(int page, string errorMessage)
        {
            EnqueueStatus(page, "❌");
            UpdateProgressBar();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nОшибка на странице {page}: {errorMessage}");
            Console.ResetColor();
        }

        public void WriteCriticalError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n🔥 КРИТИЧЕСКАЯ ОШИБКА: {errorMessage}");
            Console.ResetColor();
        }

        public void WriteTopParticipants(IEnumerable<Participant> topParticipants)
        {
            Console.WriteLine("\n");
            Console.WriteLine("┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                  🏆 ТОП-10 УЧАСТНИКОВ 🏆               │");
            Console.WriteLine("├──────┬───────────────────────┬───────────┬─────────────┤");
            Console.WriteLine("│ Место│ Участник              │ Голоса    │ Страница    │");
            Console.WriteLine("├──────┼───────────────────────┼───────────┼─────────────┤");

            foreach (var p in topParticipants.OrderBy(p => p.GlobalRank))
            {
                Console.WriteLine($"│ {p.GlobalRank,-4} │ {Truncate(p.Name, 20),-21} │ {p.Votes,-9} │ {p.Page,-11} │");
            }

            Console.WriteLine("└──────┴───────────────────────┴───────────┴─────────────┘");
        }

        public void WriteTargetParticipantInfo(Participant targetParticipant, List<Participant> allParticipants)
        {
            if (targetParticipant == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n⚠️ Целевая работа не найдена!");
                Console.ResetColor();
                return;
            }

            var rank = $"{targetParticipant.GlobalRank}/{allParticipants.Count}";

            Console.WriteLine("\n");
            Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                     🔍 РЕЗУЛЬТАТ ДЛЯ ВАШЕЙ РАБОТЫ 🔍         │");
            Console.WriteLine("├──────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ Название:      {Truncate(targetParticipant.Name, 45),-45} │");
            Console.WriteLine($"├──────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ Голоса:        {targetParticipant.Votes,-45} │");
            Console.WriteLine($"│ Страница:      {targetParticipant.Page,-45} │");
            Console.WriteLine($"│ Позиция:       {targetParticipant.PositionOnPage + " на странице",-45} │");
            Console.WriteLine($"│ Рейтинг:       {rank,-45} │");
            Console.WriteLine($"├──────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ Ссылка:        {Truncate(targetParticipant.WorkUrl, 45),-45} │");
            Console.WriteLine("└──────────────────────────────────────────────────────────────┘");
        }

        public void WriteExitPrompt()
        {
            Console.WriteLine("\n\nНажмите любую клавишу для выхода...");
        }

        public void WriteOperationCanceled()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\n⛔ Операция прервана пользователем");
            Console.ResetColor();
        }

        private void UpdateProgressBar()
        {
            if (_progressAreaTop < 0) return;
            if (_isComplete) return;

            int currentCursorTop = Console.CursorTop;
            int currentCursorLeft = Console.CursorLeft;

            Console.SetCursorPosition(0, _progressAreaTop);
            Console.WriteLine($"📊 Прогресс обработки: {_currentProgress} страниц");
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.WriteLine(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(0, _progressAreaTop + 1);
            Console.WriteLine("Статусы страниц (последние 10):");

            var recentStatuses = _recentPageStatuses.Reverse().ToList();
            var statusLines = recentStatuses.Select(s => $"Стр. {s.page}: {s.status}").ToList();

            if (statusLines.Any())
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.SetCursorPosition(0, _progressAreaTop + 2 + i);
                    if (i * 5 < statusLines.Count)
                    {
                        var line = string.Join("   ", statusLines.Skip(i * 5).Take(5));
                        Console.Write(line);
                        Console.Write(new string(' ', Console.WindowWidth - line.Length));
                    }
                    else
                    {
                        Console.Write(new string(' ', Console.WindowWidth));
                    }
                }
            }
            else
            {
                Console.SetCursorPosition(0, _progressAreaTop + 2);
                Console.Write("Ожидание данных...");
                Console.Write(new string(' ', Console.WindowWidth - "Ожидание данных...".Length));
                Console.SetCursorPosition(0, _progressAreaTop + 3);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
        }

        private void EnqueueStatus(int page, string status)
        {
            _recentPageStatuses.Enqueue((page, status));
            if (_recentPageStatuses.Count > MaxRecentStatuses)
            {
                _recentPageStatuses.Dequeue();
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Length <= maxLength
                    ? value
                    : value[..(maxLength - 3)] + "...";
        }
    }
}