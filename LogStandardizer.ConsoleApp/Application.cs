using LogStandardizer.BLL.Logic.Contracts;

namespace LogStandardizer.ConsoleApp
{
    public class Application
    {
        private readonly ILogStandardizer _standardizer;

        public Application(ILogStandardizer standardizer)
        {
            _standardizer = standardizer;
        }

        // <summary>
        /// Запуск приложения.
        /// </summary>
        public async Task RunAsync()
        {
        }
    }
}
