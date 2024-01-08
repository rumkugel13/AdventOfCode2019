using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace AdventOfCode18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var input = File.ReadAllLines("data.txt");
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

            Dictionary<char, Dictionary<char, DistanceItem>> distances = new Dictionary<char, Dictionary<char, DistanceItem>>();
            foreach (var key in pointsOfInterest.Keys)
            {
                if (char.IsUpper(pointsOfInterest[key]))
                    continue;
                distances[pointsOfInterest[key]] = FindDistances(input, pointsOfInterest, key);
            }

            int minSteps = ShortestPath(distances);
            Console.WriteLine(minSteps);
        }

        static int ShortestPath(Dictionary<char, Dictionary<char, DistanceItem>> distances)
        {
            int result = int.MaxValue;
            Dictionary<string, int> totalDistances = new Dictionary<string, int>();
            //Queue<State> queue = new Queue<State>();
            //queue.Enqueue(new State() { Character = '@', visited = new HashSet<char>() });
            BucketHeap<State> queue = new BucketHeap<State> { buckets = new Dictionary<int, List<State>>() };
            queue.Insert(new State() { Character = '@', visited = new HashSet<char>() }, 0);

            while (queue.HasBuckets())
            //while (queue.Count > 0)
            {
                //var currentState = queue.Dequeue();
                var currentState = queue.PopMin();

                if (currentState.visited.Count == distances.Count - 1)
                {
                    result = Math.Min(result, currentState.Distance);
                    //Console.WriteLine(currentState.Distance);
                    break;
                }

                var currentKey = new List<char>(currentState.visited);
                currentKey.Remove(currentState.Character);
                currentKey.Sort();
                currentKey.Add(currentState.Character);
                var stringKey = new string(currentKey.ToArray());
                if (totalDistances.TryGetValue(stringKey, out int value) && value <= currentState.Distance)
                {
                    continue;
                }
                else
                {
                    totalDistances[stringKey] = currentState.Distance;
                }

                foreach (var distanceItem in distances[currentState.Character])
                {
                    var character = distanceItem.Value.Character;

                    if (currentState.visited.Contains(character))
                    {
                        continue;
                    }

                    foreach (char door in distanceItem.Value.Doors)
                    {
                        if (!currentState.visited.Contains(char.ToLower(door)))
                        {
                            goto outerContinue;
                        }
                    }

                    HashSet<char> visited2 = new HashSet<char>(currentState.visited) { character };
                    //Console.WriteLine(string.Join("\t", visited2));

                    var newState = new State() { Character = character, visited = visited2, Distance = currentState.Distance + distanceItem.Value.Distance };
                    //queue.Enqueue(newState);
                    queue.Insert(newState, newState.Distance);

                outerContinue:
                    continue;
                }
            }

            return result;
        }

        struct Seen
        {
            public char Character;
            public string visited;
        }

        struct State
        {
            public char Character;
            public HashSet<char> visited;
            public string key;
            public int Distance;
        }

        struct DistanceItem
        {
            public int Distance;
            public char Character;
            public List<char> Doors;
        }

        static Dictionary<char, DistanceItem> FindDistances(string[] map, Dictionary<Point, char> pointsOfInterest, Point start)
        {
            Dictionary<char, DistanceItem> distances = new Dictionary<char, DistanceItem>();
            HashSet<Point> visited = new HashSet<Point>
            {
                start
            };

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

                    if (pointsOfInterest.ContainsKey(newPoint))
                    {
                        DistanceItem d = new DistanceItem();
                        d.Distance = item.Distance;
                        d.Character = pointsOfInterest[newPoint];
                        d.Doors = new List<char>(item.Doors);
                        if (char.IsUpper(pointsOfInterest[newPoint]))
                            item.Doors.Add(pointsOfInterest[newPoint]);
                        else if (pointsOfInterest[newPoint] != '@')
                            distances[pointsOfInterest[newPoint]] = d;
                    }
                    //else
                    {
                        queue.Enqueue(item);
                    }
                }
            }

            return distances;
        }

        struct QueueItem
        {
            public Point Point;
            public int Distance;
            public HashSet<char> Doors;

            public QueueItem(Point point, int v)
            {
                Point = point;
                Distance = v;
                Doors = new HashSet<char>();
            }
        }

        struct BucketHeap<T>
        {
            public Dictionary<int, List<T>> buckets;
            int minBucket;

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
