using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AdventOfCode18
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var input = File.ReadAllLines("data.txt");
            Dictionary<Point, char> pointsOfInterest = GetPointsOfInterest(input);

            Dictionary<char, List<DistanceItem>> distances = new Dictionary<char, List<DistanceItem>>();
            foreach (var poi in pointsOfInterest)
            {
                if (char.IsUpper(poi.Value))
                    continue;
                distances[poi.Value] = FindDistances(input, pointsOfInterest, poi.Key);
            }

            int minSteps = ShortestPath(distances, new char[] { '@' });
            Console.WriteLine(minSteps);

            var part2 = MakePart2(input);

            //part2 = new string[]
            //{
            //    "#######",
            //    "#a.#Cd#",
            //    "##1#2##",
            //    "#######",
            //    "##3#4##",
            //    "#cB#Ab#",
            //    "#######",
            //};

            //part2 = new string[]
            //{
            //    "###############",
            //    "#d.ABC.#.....a#",
            //    "######1#2######",
            //    "###############",
            //    "######3#4######",
            //    "#b.....#.....c#",
            //    "###############",
            //};

            //part2 = new string[]
            //{
            //    "#############",
            //    "#DcBa.#.GhKl#",
            //    "#.###1#2#I###",
            //    "#e#d#####j#k#",
            //    "###C#3#4###J#",
            //    "#fEbA.#.FgHi#",
            //    "#############",
            //};

            //part2 = new string[]
            //{
            //    "#############",
            //    "#g#f.D#..h#l#",
            //    "#F###e#E###.#",
            //    "#dCba1#2BcIJ#",
            //    "#############",
            //    "#nK.L3#4G...#",
            //    "#M###N#H###.#",
            //    "#o#m..#i#jk.#",
            //    "#############",
            //};

            Dictionary<Point, char> pointsOfInterest2 = GetPointsOfInterest(part2);

            Dictionary<char, List<DistanceItem>> distances2 = new Dictionary<char, List<DistanceItem>>();
            foreach (var poi in pointsOfInterest2)
            {
                if (char.IsUpper(poi.Value))
                    continue;
                distances2[poi.Value] = FindDistances(part2, pointsOfInterest2, poi.Key);
            }

            int minSteps2 = ShortestPath(distances2, new char[] { '1', '2', '3', '4' });
            Console.WriteLine(minSteps2);
        }

        private static Dictionary<Point, char> GetPointsOfInterest(string[] input)
        {
            Dictionary<Point, char> pointsOfInterest = new Dictionary<Point, char>();
            for (int row = 0; row < input.Length; row++)
            {
                for (int col = 0; col < input[row].Length; col++)
                {
                    var character = input[row][col];
                    if (character != '#' && character != '.')
                    {
                        pointsOfInterest[new Point(col, row)] = character;
                    }
                }
            }
            return pointsOfInterest;
        }

        private static string[] MakePart2(string[] input)
        {
            string[] newMap = (string[])input.Clone();

            Point point = new Point();
            for (int row = 0; row < input.Length; row++)
            {
                for (int col = 0; col < input[row].Length; col++)
                {
                    if (input[row][col] == '@')
                    {
                        point = new Point(col, row);
                        break;
                    }
                }
            }

            var centerLine = input[point.Y].ToCharArray();
            centerLine[point.X - 1] = '#';
            centerLine[point.X] = '#';
            centerLine[point.X + 1] = '#';
            newMap[point.Y] = new string(centerLine);

            char index = '1';
            var upperLine = input[point.Y - 1].ToCharArray();
            upperLine[point.X - 1] = index++;
            upperLine[point.X] = '#';
            upperLine[point.X + 1] = index++;
            newMap[point.Y - 1] = new string(upperLine);
            var lowerLine = input[point.Y + 1].ToCharArray();
            lowerLine[point.X - 1] = index++;
            lowerLine[point.X] = '#';
            lowerLine[point.X + 1] = index++;
            newMap[point.Y + 1] = new string(lowerLine);

            return newMap;
        }

        private static int ShortestPath(Dictionary<char, List<DistanceItem>> distances, char[] start)
        {
            int result = int.MaxValue;
            Dictionary<string, int> totalDistances = new Dictionary<string, int>();
            //Queue<State> queue = new Queue<State>();
            //queue.Enqueue(new State() { Character = start, visited = new HashSet<char>() });
            BucketHeap<State> queue = new BucketHeap<State> { buckets = new Dictionary<int, List<State>>() };
            queue.Insert(new State() { Robots = start, KeysFound = new HashSet<char>() }, 0);

            while (queue.HasBuckets())
            //while (queue.Count > 0)
            {
                //var currentState = queue.Dequeue();
                var currentState = queue.PopMin();

                if (currentState.KeysFound.Count == distances.Count - start.Length)
                {
                    result = Math.Min(result, currentState.Distance);
                    //Console.WriteLine(currentState.Distance);
                    break;
                }

                var currentKey = new List<char>(currentState.KeysFound);
                currentKey.Sort();
                currentKey.AddRange(currentState.Robots);
                var stringKey = new string(currentKey.ToArray());

                if (totalDistances.TryGetValue(stringKey, out int value) && value <= currentState.Distance)
                {
                    continue;
                }
                else
                {
                    totalDistances[stringKey] = currentState.Distance;
                }

                for (int i = 0; i < start.Length; i++)
                {
                    foreach (var distanceItem in distances[currentState.Robots[i]])
                    {
                        var character = distanceItem.Character;

                        if (currentState.KeysFound.Contains(character))
                        {
                            continue;
                        }

                        foreach (char door in distanceItem.Doors)
                        {
                            if (!currentState.KeysFound.Contains(char.ToLower(door)))
                            {
                                goto outerContinue;
                            }
                        }

                        HashSet<char> nextKeyFound = new HashSet<char>(currentState.KeysFound) { character };
                        char[] nextRobots = (char[])currentState.Robots.Clone();
                        nextRobots[i] = character;

                        var newState = new State() { Robots = nextRobots, KeysFound = nextKeyFound, Distance = currentState.Distance + distanceItem.Distance };
                        //queue.Enqueue(newState);
                        queue.Insert(newState, newState.Distance);

                    outerContinue:
                        continue;
                    }
                }
            }

            return result;
        }

        private struct State
        {
            public char[] Robots;
            public HashSet<char> KeysFound;
            public int Distance;
        }

        private struct DistanceItem
        {
            public int Distance;
            public char Character;
            public List<char> Doors;
        }

        private static List<DistanceItem> FindDistances(string[] map, Dictionary<Point, char> pointsOfInterest, Point start)
        {
            List<DistanceItem> distances = new List<DistanceItem>();
            HashSet<Point> visited = new HashSet<Point> { start };

            Queue<QueueItem> queue = new Queue<QueueItem>();
            queue.Enqueue(new QueueItem(start, 0));

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (Point dir in new Point[4] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    var newPoint = new Point(current.Point.X + dir.X, current.Point.Y + dir.Y);

                    if (visited.Contains(newPoint) || map[newPoint.Y][newPoint.X] == '#')
                    {
                        continue;
                    }

                    visited.Add(newPoint);
                    var item = new QueueItem(newPoint, current.Distance + 1);
                    item.Doors.UnionWith(current.Doors);

                    if (pointsOfInterest.TryGetValue(newPoint, out char character))
                    {
                        DistanceItem d = new DistanceItem
                        {
                            Distance = item.Distance,
                            Character = character,
                            Doors = new List<char>(item.Doors)
                        };
                        if (char.IsUpper(character))
                            item.Doors.Add(character);
                        else if (!"@1234".Contains(character))
                            distances.Add(d);
                    }

                    queue.Enqueue(item);
                }
            }

            return distances;
        }

        private struct QueueItem
        {
            public Point Point;
            public int Distance;
            public HashSet<char> Doors;

            public QueueItem(Point point, int distance)
            {
                Point = point;
                Distance = distance;
                Doors = new HashSet<char>();
            }
        }

        private struct BucketHeap<T>
        {
            public Dictionary<int, List<T>> buckets;
            private int minBucket;

            public bool HasBuckets()
            {
                return buckets.Count > 0;
            }

            public void Insert(T state, int minDist)
            {
                if (!buckets.ContainsKey(minDist))
                    buckets[minDist] = new List<T>();

                buckets[minDist].Add(state);
                if (minDist < minBucket)
                {
                    minBucket = minDist;
                }
            }

            public T PopMin()
            {
                T state = buckets[minBucket][0];
                buckets[minBucket].RemoveAt(0);

                if (buckets[minBucket].Count == 0)
                {
                    buckets.Remove(minBucket);
                    minBucket = int.MaxValue;
                    foreach (var bucket in buckets)
                    {
                        if (bucket.Key < minBucket)
                        {
                            minBucket = bucket.Key;
                        }
                    }
                }

                return state;
            }
        }
    }
}