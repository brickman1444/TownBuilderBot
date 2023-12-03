using System.Collections.Generic;

namespace TownBuilderBot
{
    static class CollectionUtils
    {
        public static List<T> RandomFirstN<T>(int n, IEnumerable<T> inArray, System.Random rand)
        {
            List<T> list = new List<T>(inArray);

            List<T> output = new List<T>();

            for (int i = 0; i < n; i++)
            {
                int randomIndex = rand.Next(list.Count);
                output.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }

            return output;
        }
    }
}