using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode14
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Ores> recipes = new List<Ores>();

            using (StreamReader sr = new StreamReader(File.OpenRead("data.txt")))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string[] inOut = s.Split("=>");

                    // output first
                    Ores ore = new Ores();
                    inOut[1] = inOut[1].Trim();
                    string[] produces = inOut[1].Split(' ');
                    ore.Amount = Convert.ToInt32(produces[0]);
                    ore.Name = produces[1];

                    // then input
                    ore.Requires = new List<Ores>();

                    string[] inList = inOut[0].Split(',');

                    for (int i = 0; i < inList.Length; i++)
                    {
                        inList[i] = inList[i].Trim();
                        string[] needed = inList[i].Split(' ');
                        Ores required = new Ores();
                        required.Amount = Convert.ToInt32(needed[0]);
                        required.Name = needed[1];
                        ore.Requires.Add(required);
                    }

                    recipes.Add(ore);
                }
            }

            Console.WriteLine($"Number of reactions: {recipes.Count}");

            // part1 | result: 485720

            long ores = GetOres(recipes, 1);

            Console.WriteLine($"ORE count: {ores}");

            // part2 | result: 3848998

            long maxOres = 1000000000000;
            long fuelAmount = GetFuel(recipes, maxOres);

            Console.WriteLine($"FUEL amount: {fuelAmount}");

            Console.ReadKey();
        }

        static long GetFuel(List<Ores> recipes, long maxOre)
        {
            long min = 1;
            long max = 2;

            Console.WriteLine("Looking for upper bound");

            while (GetOres(recipes, max) < maxOre)
            {
                //Console.WriteLine($"Current max: {max}");
                max *= 2;
            }

            Console.WriteLine("Upper bound found " + max);

            if (GetOres(recipes, max) == maxOre)
            {
                return max;
            }

            while (max > min + 1)
            {
                long mid = (long)Math.Floor(min + (max - min)/2d);
                long ores = GetOres(recipes, mid);

                if (ores > maxOre)
                {
                    max = mid;
                }
                else if (ores == maxOre)
                {
                    return min;
                }
                else
                {
                    min = mid;
                }

            }
            return min;
        }

        static long GetOres(List<Ores> recipes, long needAmount)
        {
            Dictionary<string, long> need = new Dictionary<string, long>
            {
                { "FUEL", needAmount }
            };

            Dictionary<string, long> have = new Dictionary<string, long>();

            while (need.Any(kvp => kvp.Key != "ORE" && kvp.Value > 0))
            {
                var next = need.First(kvp => kvp.Key != "ORE" && kvp.Value > 0);    //ore doesn't have recipe
                Ores reaction = recipes.First(o => o.Name == next.Key);

                long modifier = (long)Math.Ceiling(need[next.Key] / (double)reaction.Amount); //create at least the amount we need

                RemoveNeed(need, have, next.Key, modifier * reaction.Amount);

                foreach (Ores ore in reaction.Requires)
                {
                    AddNeed(need, have, ore.Name, modifier * ore.Amount);
                }
            }

            return need["ORE"];
        }

        static void RemoveNeed(Dictionary<string, long> need, Dictionary<string, long> have, string type, long amount)
        {
            need.TryGetValue(type, out long currentNeed);
            have.TryGetValue(type, out long currentHave);

            long delta = currentHave - currentNeed + amount;

            if (delta == 0)
            {
                need[type] = 0;
                have[type] = 0;
            }
            else if (delta > 0)
            {
                have[type] = delta;
                need[type] = 0;
            }
            else
            {
                need[type] = -delta;
                have[type] = 0;
            }
        }

        static void AddNeed(Dictionary<string, long> need, Dictionary<string, long> have, string type, long amount)
        {
            need.TryGetValue(type, out long currentNeed);
            have.TryGetValue(type, out long currentHave);

            long delta = currentHave - currentNeed - amount;

            if (delta == 0)
            {
                need[type] = 0;
                have[type] = 0;
            }
            else if (delta > 0)
            {
                have[type] = delta;
                need[type] = 0;
            }
            else
            {
                need[type] = -delta;
                have[type] = 0;
            }
        }

        struct Ores
        {
            public long Amount;
            public string Name;
            public List<Ores> Requires;
        }
    }
}
