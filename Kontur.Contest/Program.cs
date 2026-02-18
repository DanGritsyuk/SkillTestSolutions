namespace Kontur.Contest
{
    class Program
    {
        static void Main()
        {
            int n = int.Parse(Console.ReadLine()!);
            int[] c = new int[n];
            int[] m = new int[n];

            const int MAX = 110;
            int Vmax = 0;

            int[] freqAll = new int[MAX + 1];

            int totalCats = 0;
            for (int i = 0; i < n; i++)
            {
                var parts = Console.ReadLine()!.Split().Select(int.Parse).ToArray();
                c[i] = parts[0];
                m[i] = parts[1];
                freqAll[c[i]]++;
                totalCats += c[i];
                if (c[i] > Vmax) Vmax = c[i];
            }

            int bestCost = int.MaxValue;
            int bestIndex = -1;
            int[] bestFinal = null!;

            int[] prefCountAll = new int[MAX + 2];
            int[] prefSumAll = new int[MAX + 2];

            prefCountAll[Vmax + 1] = 0;
            prefSumAll[Vmax + 1] = 0;
            for (int v = Vmax; v >= 0; v--)
            {
                prefCountAll[v] = prefCountAll[v + 1] + freqAll[v];
                prefSumAll[v] = prefSumAll[v + 1] + freqAll[v] * v;
            }

            for (int i = 0; i < n; i++)
            {
                if (m[i] == -1) continue;

                int ci = c[i];
                int sumOthers = totalCats - ci;

                int Tmax = ci + sumOthers;

                int chosenT = -1;
                for (int T = ci; T <= Tmax; T++)
                {
                    int prefCount = (T <= Vmax) ? prefCountAll[T] : 0;
                    int prefSum = (T <= Vmax) ? prefSumAll[T] : 0;
                    if (ci >= T)
                    {
                        prefCount -= 1;
                        prefSum -= ci;
                    }

                    int R = prefSum - (T - 1) * prefCount;

                    int t = T - ci;
                    if (t >= R)
                    {
                        chosenT = T;
                        break;
                    }
                }

                if (chosenT == -1)
                {
                    continue;
                }

                int tNeeded = chosenT - ci;
                int totalCost = m[i] + tNeeded;
                if (totalCost < bestCost)
                {
                    bestCost = totalCost;
                    bestIndex = i;

                    int[] final = new int[n];
                    Array.Copy(c, final, n);

                    int rem = tNeeded;

                    var pairs = new List<(int val, int idx)>();
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i) continue;
                        pairs.Add((c[j], j));
                    }
                    pairs.Sort((a, b) => b.val.CompareTo(a.val));

                    for (int p = 0; p < pairs.Count && rem > 0; p++)
                    {
                        var (val, idx) = pairs[p];
                        int take = Math.Min(val, rem);
                        final[idx] = val - take;
                        rem -= take;
                    }

                    final[i] = ci + tNeeded;

                    bestFinal = final;
                }
            }

            Console.WriteLine(bestCost);
            Console.WriteLine(bestIndex + 1);
            Console.WriteLine(string.Join(" ", bestFinal));
        }
    }
}
