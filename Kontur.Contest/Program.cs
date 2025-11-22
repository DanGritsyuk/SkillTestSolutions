namespace Kontur.Contest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            long answer = CountParts(Console.ReadLine()!);

            Console.WriteLine(answer);
        }

        private static long CountParts(string t)
        {
            long countM = 0;
            foreach (char c in t)
            {
                if (c == 'M')
                    countM++;
            }

            // Решаем неравенство: n(n+1)/2 <= countM
            // n^2 + n - 2 * countM <= 0
            // Используем формулу для нахождения корней квадратного уравнения
            long n = (long)((-1 + Math.Sqrt(1 + 8 * countM)) / 2);
            return n;
        }
    }
}
