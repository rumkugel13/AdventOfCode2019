using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string[] orbitData;
            int result1 = 0, result2 = 0;

            using (StreamReader streamReader = new StreamReader(File.OpenRead("data.txt")))
            {
                orbitData = streamReader.ReadToEnd().Split("\r\n");
            }

            Dictionary<string, List<string>> objects = new Dictionary<string, List<string>>();

            foreach (string s in orbitData)
            {
                string[] orbits = s.Split(')'); // 1 orbits 0

                if (!objects.ContainsKey(orbits[0]))
                {
                    objects.Add(orbits[0], new List<string>());
                }

                objects[orbits[0]].Add(orbits[1]);
            }

            // calulate orbitcount direct and indirect

            int count = 0;
            CountOrbits(objects, "COM", 0, ref count);
            result1 = count;

            // calculate min steps from YOU to SAN (orbited objects)

            // note: first parent is index 0, which is correct for the calculation
            List<string> youparents = GetAllParents(objects, "YOU"), sanparents = GetAllParents(objects, "SAN");

            int depth1 = 0, depth2 = 0;

            for (int i = 0; i < youparents.Count; i++)
            {
                int index = sanparents.IndexOf(youparents[i]);

                if (index >= 0)
                {
                    depth1 = i;
                    depth2 = index;
                    break;
                }
            }

            result2 = depth1 + depth2;

            Console.WriteLine($"Result: {result1} , {result2}");
            Console.ReadKey();
        }

        static List<string> GetAllParents(Dictionary<string, List<string>> objects, string child)
        {
            List<string> parents = new List<string>();

            string parent;
            string currentChild = child;

            while (true)
            {
                parent = GetParent(objects, currentChild);
                if (string.IsNullOrEmpty(parent))
                {
                    break;
                }
                
                parents.Add(parent);
                currentChild = parent;
            };

            return parents;
        }

        static string GetParent(Dictionary<string, List<string>> objects, string child)
        {
            foreach (string s in objects.Keys)
            {
                if (objects[s].Contains(child))
                {
                    return s;
                }
            }

            return default;
        }

        static void CountOrbits(Dictionary<string, List<string>> objects, string start, int depth, ref int result)
        {
            if (objects.ContainsKey(start))
            {
                foreach (string s in objects[start])
                {
                    CountOrbits(objects, s, depth + 1, ref result);
                }
            }
            result += depth;
        }
    }
}
