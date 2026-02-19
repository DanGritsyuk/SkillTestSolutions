namespace Learn.HelloWorld
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ПРИМИТИВНЫЕ ТИПЫ ДАННЫХ В C# ===\n");

            // ========== ЦЕЛОЧИСЛЕННЫЕ ТИПЫ ==========
            Console.WriteLine("--- ЦЕЛЫЕ ЧИСЛА ---");

            // byte - от 0 до 255 (1 байт)
            byte studentAge = 20;
            Console.WriteLine($"byte: {studentAge} (от 0 до 255, 1 байт)");

            // sbyte - от -128 до 127 (1 байт, знаковый)
            sbyte temperature = -15;
            Console.WriteLine($"sbyte: {temperature} (от -128 до 127, 1 байт)");

            // short - от -32,768 до 32,767 (2 байта)
            short population = 32000;
            Console.WriteLine($"short: {population} (от -32,768 до 32,767, 2 байта)");

            // ushort - от 0 до 65,535 (2 байта, беззнаковый)
            ushort distance = 65000;
            Console.WriteLine($"ushort: {distance} (от 0 до 65,535, 2 байта)");

            // int - самый популярный, от -2,147,483,648 до 2,147,483,647 (4 байта)
            int studentCount = 1500;
            Console.WriteLine($"int: {studentCount} (от -2 млрд до 2 млрд, 4 байта)");

            // uint - от 0 до 4,294,967,295 (4 байта)
            uint positiveNumber = 4000000000;
            Console.WriteLine($"uint: {positiveNumber} (от 0 до 4 млрд, 4 байта)");

            // long - от -9,223,372,036,854,775,808 до 9,223,372,036,854,775,807 (8 байт)
            long worldPopulation = 7_900_000_000;
            Console.WriteLine($"long: {worldPopulation} (очень большие числа, 8 байт)");

            // ulong - от 0 до 18,446,744,073,709,551,615 (8 байт)
            ulong bigUnsigned = 18_000_000_000_000_000_000;
            Console.WriteLine($"ulong: {bigUnsigned} (огромные беззнаковые, 8 байт)");

            // ========== ЧИСЛА С ПЛАВАЮЩЕЙ ТОЧКОЙ ==========
            Console.WriteLine("\n--- ДРОБНЫЕ ЧИСЛА ---");

            // float - 4 байта, точность ~6-9 знаков (нужен суффикс f)
            float pi = 3.1415926535f;
            Console.WriteLine($"float: {pi} (4 байта, точность ~7 знаков)");

            // double - 8 байт, точность ~15-17 знаков (по умолчанию для дробей)
            double precisePi = 3.141592653589793;
            Console.WriteLine($"double: {precisePi} (8 байт, точность ~15 знаков, используется по умолчанию)");

            // decimal - 16 байт, максимальная точность для финансовых расчетов (нужен суффикс m)
            decimal moneyAmount = 199.99m;
            Console.WriteLine($"decimal: {moneyAmount} (16 байт, точность для денег)");

            // ========== СИМВОЛЬНЫЙ ТИП ==========
            Console.WriteLine("\n--- СИМВОЛЫ ---");

            // char - один символ в одинарных кавычках (2 байта, Unicode)
            char grade = 'A';
            char symbol = '@';
            char digit = '5';
            Console.WriteLine($"char: '{grade}' (2 байта, один символ Unicode)");

            // ========== ЛОГИЧЕСКИЙ ТИП ==========
            Console.WriteLine("\n--- ЛОГИЧЕСКИЙ ТИП ---");

            // bool - true или false (1 байт, но реально занимает 1 байт)
            bool isStudent = true;
            bool passedExam = false;
            Console.WriteLine($"bool: {isStudent} (может быть true или false)");

            // ========== СТРОКОВЫЙ ТИП ==========
            Console.WriteLine("\n--- СТРОКИ (НЕ ПРИМИТИВ, НО ВАЖНО) ---");

            // string - ссылочный тип, но очень часто используется
            string name = "Иван Петров";
            string message = "Привет, мир!";
            Console.WriteLine($"string: \"{name}\" (ссылочный тип, последовательность символов)");

            // ========== ДЕМОНСТРАЦИЯ ОСОБЕННОСТЕЙ ==========
            Console.WriteLine("\n=== ОСОБЕННОСТИ ТИПОВ ===\n");

            // Неявная типизация (var)
            Console.WriteLine("--- var (неявная типизация) ---");
            var autoInt = 42;              // компилятор поймет, что это int
            var autoDouble = 3.14;          // компилятор поймет, что это double
            var autoString = "Автострока";   // компилятор поймет, что это string
            Console.WriteLine($"var autoInt = {autoInt} -> тип: {autoInt.GetType()}");
            Console.WriteLine($"var autoDouble = {autoDouble} -> тип: {autoDouble.GetType()}");
            Console.WriteLine($"var autoString = {autoString} -> тип: {autoString.GetType()}");

            // Суффиксы для явного указания типа
            Console.WriteLine("\n--- Суффиксы для литералов ---");
            float f = 3.14f;        // f или F для float
            double d = 3.14d;        // d или D для double (необязательно)
            decimal m = 3.14m;        // m или M для decimal
            uint u = 42u;             // u или U для uint
            long l = 42L;             // l или L для long
            ulong ul = 42UL;          // UL для ulong
            Console.WriteLine("float f = 3.14f;  // суффикс f");
            Console.WriteLine("decimal m = 3.14m; // суффикс m для денег");

            // Проверка размеров типов
            Console.WriteLine("\n--- Размеры типов в байтах ---");
            Console.WriteLine($"byte: {sizeof(byte)} байт");
            Console.WriteLine($"short: {sizeof(short)} байт");
            Console.WriteLine($"int: {sizeof(int)} байт");
            Console.WriteLine($"long: {sizeof(long)} байт");
            Console.WriteLine($"float: {sizeof(float)} байт");
            Console.WriteLine($"double: {sizeof(double)} байт");
            Console.WriteLine($"decimal: {sizeof(decimal)} байт");
            Console.WriteLine($"char: {sizeof(char)} байт");
            Console.WriteLine($"bool: {sizeof(bool)} байт");

            // Преобразование типов
            Console.WriteLine("\n--- Преобразование типов ---");

            // Неявное преобразование (без потери данных)
            int intValue = 100;
            long longValue = intValue;  // int неявно в long
            Console.WriteLine($"Неявное: int {intValue} -> long {longValue}");

            // Явное преобразование (возможна потеря данных)
            double doubleValue = 123.456;
            int truncatedInt = (int)doubleValue;  // (int) - приведение типов
            Console.WriteLine($"Явное: double {doubleValue} -> int {truncatedInt} (потеря дробной части)");

            // Конвертация через класс Convert
            string numberString = "42";
            int parsedInt = Convert.ToInt32(numberString);
            Console.WriteLine($"Конвертация: string \"{numberString}\" -> int {parsedInt}");

            Console.WriteLine("\n=== КОНЕЦ ДЕМОНСТРАЦИИ ===");
            Console.ReadLine(); // Чтобы консоль не закрывалась
        }
    }
}
