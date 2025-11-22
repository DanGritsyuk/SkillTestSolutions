namespace Kontur.Contest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var k = int.Parse(Console.ReadLine()!);
            var numbers = new List<string>();

            for (int i = 0; i < k; i++)
            {
                numbers.Add(Console.ReadLine()!);
            }

            string result = GetStrLargestNumber(numbers);
            Console.WriteLine(result);


        }

        static string GetStrLargestNumber(List<string> numbers)
        {
            numbers.Sort((a, b) =>
            {
                string ab = a + b;
                string ba = b + a;
                return ba.CompareTo(ab); // Обратный порядок для получения убывающей сортировки
            });

            string result = string.Concat(numbers);

            // Проверка на случай, если все числа нули
            if (result.Length > 0 && result[0] == '0')
            {
                return "0";
            }

            return result;
        }
    }
}
