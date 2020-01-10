using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode10
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Point> asteroids = new List<Point>();

            using (StreamReader sr = new StreamReader(File.OpenRead("data.txt")))
            {
                int y = 0;
                while (!sr.EndOfStream)
                {
                    string input = sr.ReadLine();
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i] == '#')
                        {
                            asteroids.Add(new Point(i, y));
                        }
                    }
                    y++;
                }
            }

            // part1

            int result1 = 0;
            Point station = Point.Empty;

            foreach (Point p in asteroids)
            {
                HashSet<float> angles = new HashSet<float>(asteroids.Except(new Point[] { p })
                                                                    .Select(p2 => (float)Math.Atan2(p.Y - p2.Y, p.X - p2.X)));

                if (angles.Count > result1)
                {
                    result1 = angles.Count;
                    station = p;
                }
            }

            // part2

            // store all points by their angle
            Dictionary<double, List<Point>> pointsByAngle = new Dictionary<double, List<Point>>();
            const double halfpi = Math.PI / 2;
            const double twopi = Math.PI * 2;

            foreach (Point p2 in asteroids.Except(new Point[] { station }))
            {
                double angle = Math.Atan2(-(station.Y - p2.Y), station.X - p2.X)/* + halfpi*/;

                // wrap angle to -p .. 0 .. +pi
                //angle %= twopi;
                //if (angle > Math.PI) angle -= twopi;
                //else if (angle < -Math.PI) angle += twopi;

                if (!pointsByAngle.ContainsKey(angle))
                {
                    pointsByAngle[angle] = new List<Point>();
                }

                pointsByAngle[angle].Add(p2);
            }

            List<double> sortedAngles = pointsByAngle.Keys.ToList();
            sortedAngles.Sort();
            sortedAngles.Reverse(); //reverse list, since atan2 calculates counter clockwise

            //split list where angle < -halfpi and switch order, since objective starts at up, not right (angle -halfpi instead of 0)


            int splitat = 0;
            for (int i = 0; i < sortedAngles.Count; i++)
            {
                if (sortedAngles[i] <= -halfpi)
                {
                    splitat = i;
                    break;
                }
            }

            List<double> anglesList = new List<double>(sortedAngles.GetRange(splitat, sortedAngles.Count - splitat));
            IEnumerable<double> enumerable = anglesList.Concat(sortedAngles.GetRange(0, splitat));

            Console.WriteLine($"Station: {station.X},{station.Y}");
            int index = 1;
            int max = 200;
            int result2 = 0;

            while (index < max)
            {
                foreach (double a in enumerable)    //iterate all angles clockwise starting with angle pointing up
                {
                    var ordered = pointsByAngle[a].OrderBy(p => Math.Pow(p.X - station.X, 2) + Math.Pow(p.Y - station.Y, 2));
                    if (ordered.Count() < 1)
                        continue;
                    Point first = ordered.First();
                    double minDistSquared = Math.Pow(first.X - station.X, 2) + Math.Pow(first.Y - station.Y, 2);

                    Console.WriteLine($"Nearest: {index} | {first.X},{first.Y},{minDistSquared}, {a}");
                    pointsByAngle[a].Remove(first);

                    if (++index > max)
                    {
                        result2 = first.X * 100 + first.Y;
                        break;
                    }

                    //Point nearest = Point.Empty;
                    //double minDistSquared = double.MaxValue;

                    //foreach (Point p in pointsByAngle[a])//.Except(new Point[] { station }))
                    //{
                    //    double distSqrd = Math.Pow(p.X - station.X, 2) + Math.Pow(p.Y - station.Y, 2);
                    //    if (distSqrd < minDistSquared)
                    //    {
                    //        minDistSquared = distSqrd;
                    //        nearest = p;
                    //    }
                    //}

                    //if (minDistSquared != double.MaxValue)
                    //{
                    //    Console.WriteLine($"Nearest: {index} | {nearest.X},{nearest.Y},{minDistSquared}, {a}");
                    //    pointsByAngle[a].Remove(nearest);

                    //    if (++index > max)
                    //    {
                    //        result2 = nearest.X * 100 + nearest.Y;
                    //        break;
                    //    }
                    //}
                }
            }

            // Results: 247, 1919

            Console.WriteLine($"Results: {result1}, {result2}");

            Console.ReadKey();
        }
    }
}
