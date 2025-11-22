namespace Kontur.Contest
{
    class Program
    {
        static void Main()
        {
            int n = int.Parse(Console.ReadLine()!);
            (long c, long m)[] sanctuaries = new (long, long)[n];
            for (int i = 0; i < n; i++)
            {
                string[] input = Console.ReadLine()!.Split();
                long cVal = long.Parse(input[0]);
                long mVal = long.Parse(input[1]);
                sanctuaries[i] = (cVal, mVal);
            }

            long[] allC = sanctuaries.Select(s => s.c).ToArray();
            Array.Sort(allC);
            long[] sufSum = new long[allC.Length + 1];
            for (int i = allC.Length - 1; i >= 0; i--)
            {
                sufSum[i] = sufSum[i + 1] + allC[i];
            }

            long bestCost = long.MaxValue;
            int bestIndex = -1;
            long bestT = 0;

            for (int i = 0; i < n; i++)
            {
                if (sanctuaries[i].m == -1) continue;

                long c_i = sanctuaries[i].c;
                long T_max = FindTMax(c_i, allC, sufSum);

                long cost;
                if (T_max == c_i)
                {
                    long s_val = S(c_i, allC, sufSum);
                    cost = sanctuaries[i].m + s_val - 1;
                }
                else
                {
                    long s_val = S(T_max, allC, sufSum);
                    cost = sanctuaries[i].m + s_val;
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestIndex = i;
                    bestT = T_max;
                }
            }

            Console.WriteLine(bestCost);
            Console.WriteLine(bestIndex + 1);

            long[] newC = new long[n];
            for (int j = 0; j < n; j++)
            {
                if (j == bestIndex)
                {
                    if (bestT == sanctuaries[j].c)
                    {
                        newC[j] = sanctuaries[j].c + S(bestT, allC, sufSum) - 1;
                    }
                    else
                    {
                        newC[j] = sanctuaries[j].c + S(bestT, allC, sufSum);
                    }
                }
                else
                {
                    newC[j] = Math.Min(sanctuaries[j].c, bestT - 1);
                }
            }

            Console.WriteLine(string.Join(" ", newC));
        }

        private static long FindTMax(long c_i, long[] allC, long[] sufSum)
        {
            long L = c_i;
            long R = allC[allC.Length - 1] + 1;
            long T_max = c_i;
            while (L <= R)
            {
                long mid = (L + R) / 2;
                long s_val = S(mid, allC, sufSum);
                if (c_i + s_val >= mid)
                {
                    T_max = mid;
                    L = mid + 1;
                }
                else
                {
                    R = mid - 1;
                }
            }
            return T_max;
        }

        private static long S(long T, long[] allC, long[] sufSum)
        {
            int k = LowerBound(allC, T);
            int n = allC.Length;
            if (k == n) return 0;
            return sufSum[k] - (n - k) * (T - 1);
        }

        private static int LowerBound(long[] arr, long T)
        {
            int low = 0;
            int high = arr.Length;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (arr[mid] < T)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }
            return low;
        }
    }
}
