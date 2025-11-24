namespace Kontur.Contest
{
    class Program
    {
        static void Main()
        {
            int sanctuaryCount = int.Parse(Console.ReadLine());

            int[] cats = new int[sanctuaryCount];

            int bestPriority = int.MinValue;
            int chosenSanctuary = 0;
            int indexOfMaxCats = -1;
            int totalCost = 0;

            for (int i = 0; i < sanctuaryCount; i++)
            {
                var data = Console.ReadLine()!.Split().Select(int.Parse).ToArray();
                int catCount = data[0];
                int bribeCost = data[1];
                cats[i] = catCount;

                if (bribeCost >= 0)
                {
                    int currentPriority = catCount - bribeCost;

                    if (currentPriority > bestPriority)
                    {
                        bestPriority = currentPriority;
                        totalCost = bribeCost;

                        if (indexOfMaxCats > -1 && cats[chosenSanctuary] > cats[indexOfMaxCats])
                        {
                            indexOfMaxCats = chosenSanctuary;
                        }

                        chosenSanctuary = i;
                    }
                    else
                    {
                        if (indexOfMaxCats < 0 || cats[i] > cats[indexOfMaxCats])
                        {
                            indexOfMaxCats = i;
                        }
                    }
                }
                else
                {
                    if (indexOfMaxCats < 0 || cats[i] > cats[indexOfMaxCats])
                    {
                        indexOfMaxCats = i;
                    }
                }
            }

            if (indexOfMaxCats > -1 && cats[chosenSanctuary] >= cats[indexOfMaxCats])
            {
                int initialCats = cats[chosenSanctuary];
                int combinedCats = cats[chosenSanctuary] + cats[indexOfMaxCats];

                cats[chosenSanctuary] = combinedCats / 2 + 1;
                cats[indexOfMaxCats] = combinedCats - cats[chosenSanctuary];

                totalCost += cats[chosenSanctuary] - initialCats;
            }

            Console.WriteLine(totalCost);
            Console.WriteLine(chosenSanctuary + 1);
            Console.WriteLine(string.Join(" ", cats));
        }
    }
}
