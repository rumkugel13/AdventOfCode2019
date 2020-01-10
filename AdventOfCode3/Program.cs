using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AdventOfCode3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string[] path1, path2;

            using (StreamReader reader = new StreamReader(File.OpenRead("data1.txt")))
            {
                path1 = reader.ReadToEnd().Split(',');
            }

            using (StreamReader reader = new StreamReader(File.OpenRead("data2.txt")))
            {
                path2 = reader.ReadToEnd().Split(',');
            }

            Console.WriteLine("Files read, creating maps");

            List<Point> map1 = CreateMap(path1);

            Console.WriteLine("Map1 done");

            List<Point> map2 = CreateMap(path2);

            Console.WriteLine("Map2 done, finding intersections");

            List<Point> intersections = FindIntersections(map1, map2);

            Console.WriteLine("Intersections found, finding closest");

            Point closest = ClosestIntersection(intersections, new Point());

            Console.WriteLine($"Closest distance: {ManhattanDistance(closest, new Point())}");

            int shortest = ShortestPathSumToIntersection(intersections, map1, map2);

            Console.WriteLine($"Closest walk: {shortest}");

            Console.ReadKey();
        }

        static List<Point> CreateMap(string[] path)
        {
            List<Point> result = new List<Point>();
            Point pos = new Point();   //start position; central port

            foreach(string s in path)
            {
                char dir = s[0];
                int amount = Convert.ToInt32(s.Substring(1));

                switch (dir)
                {
                    case 'U':
                        for (int i = 0; i < amount; i++)
                        {
                            result.Add(pos);
                            pos.Y--;
                        }
                        break;
                    case 'D':
                        for (int i = 0; i < amount; i++)
                        {
                            result.Add(pos);
                            pos.Y++;
                        }
                        break;
                    case 'L':
                        for (int i = 0; i < amount; i++)
                        {
                            result.Add(pos);
                            pos.X--;
                        }
                        break;
                    case 'R':
                        for (int i = 0; i < amount; i++)
                        {
                            result.Add(pos);
                            pos.X++;
                        }
                        break;
                }

            }

            return result;
        }

        static List<Point> FindIntersections(List<Point> map1, List<Point> map2)
        {
            List<Point> intersections = new List<Point>();
            HashSet<Point> set1 = new HashSet<Point>(map1);

            foreach (Point p in map2)
            {
                if (set1.Contains(p))
                {
                    intersections.Add(p);
                }
            }
            return intersections;
        }

        static Point ClosestIntersection(List<Point> intersections, Point center)
        {
            int minDist = ushort.MaxValue;
            Point closest = new Point(minDist);

            foreach (Point p in intersections)
            {
                int dist = ManhattanDistance(p, center);
                if (dist < minDist && dist > 0)
                {
                    minDist = dist;
                    closest = p;
                }
            }

            return closest;
        }

        static int ShortestPathSumToIntersection(List<Point> intersections, List<Point> map1, List<Point> map2)
        {
            Point shortest;
            int smallestSum = int.MaxValue;

            foreach (Point p in intersections)
            {
                int steps1 = map1.IndexOf(p);

                int steps2 = map2.IndexOf(p);

                int sum = steps1 + steps2;

                if (sum < smallestSum && !(p.X == 0 && p.Y == 0))
                {
                    shortest = p;
                    smallestSum = sum;
                }
            }

            return smallestSum;
        }

        static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
    }
}
