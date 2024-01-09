﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AdventOfCode20
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var input = File.ReadAllLines("data.txt");
            Dictionary<Point, string> pointsOfInterest = GetPointsOfInterest(input);

            Point start = new Point();
            Point end = new Point();
            foreach (var poi in pointsOfInterest)
            {
                if (poi.Value == "AA")
                {
                    start = poi.Key;
                }
                else if (poi.Value == "ZZ")
                {
                    end = poi.Key;
                }
            }

            var labelToPortals = LabelToPortals(pointsOfInterest);

            int steps = CountSteps(input, pointsOfInterest, labelToPortals, start, end);
            Console.WriteLine(steps);
        }

        private static int CountSteps(string[] input, Dictionary<Point, string> pointsOfInterest, Dictionary<string, Point[]> labelsToPortals, Point start, Point end)
        {
            int result = int.MaxValue;
            Dictionary<Point, int> visited = new Dictionary<Point, int>();
            Queue<(Point, int, string)> queue = new Queue<(Point, int, string)>();
            queue.Enqueue((start, 0, "AA->"));

            while (queue.Count > 0)
            {
                var (currentPos, currentDist, path) = queue.Dequeue();

                if (currentPos == end)
                {
                    Console.WriteLine(path + "ZZ" + currentDist);
                    result = Math.Min(result, currentDist);
                    continue;
                }

                foreach (var dir in new Point[] {new Point(0,-1),  new Point(1,0), new Point(0,1),  new Point(-1,0)})
                {
                    Point nextPoint = new Point(currentPos.X + dir.X, currentPos.Y + dir.Y);
                    string nextPath = path;
                    int nextDist = currentDist;

                    if (input[nextPoint.Y][nextPoint.X] != '.' || (visited.TryGetValue(nextPoint, out int dist) && dist < currentDist + 1))
                    {
                        continue;
                    }

                    if (pointsOfInterest.TryGetValue(nextPoint, out string label) && labelsToPortals.ContainsKey(label))
                    {
                        nextPoint = labelsToPortals[label][0] == nextPoint ? labelsToPortals[label][1] : labelsToPortals[label][0];
                        nextDist++;
                        nextPath += label + "->";
                    }

                    visited[nextPoint] = nextDist + 1;
                    queue.Enqueue((nextPoint, nextDist + 1, nextPath));
                }
            }

            return result;
        }

        private static Dictionary<string, Point[]> LabelToPortals(Dictionary<Point, string> poi)
        {
            Dictionary<string, Point[]> labelToPortals = new Dictionary<string, Point[]>();
            foreach (var pair in poi)
            {
                foreach (var pair2 in poi)
                {
                    if (pair.Key != pair2.Key && pair.Value == pair2.Value && !labelToPortals.ContainsKey(pair.Value))
                    {
                        labelToPortals[pair.Value] = new Point[] { pair.Key, pair2.Key };
                    }
                }
            }
            return labelToPortals;
        }

        private static Dictionary<Point, string> GetPointsOfInterest(string[] input)
        {
            Dictionary<Point, string> pointsOfInterest = new Dictionary<Point, string>();
            for (int row = 1; row < input.Length - 1; row++)
            {
                for (int col = 1; col < input[row].Length - 1; col++)
                {
                    var character = input[row][col];
                    Point point = new Point(col, row);
                    if (char.IsLetter(character))
                    {
                        char up = input[row - 1][col], down = input[row + 1][col], left = input[row][col - 1], right = input[row][col + 1];

                        if (char.IsLetter(up) && down == '.')
                        {
                            pointsOfInterest[new Point(col, row + 1)] = up + "" + character;
                        }
                        else if (char.IsLetter(down) && up == '.')
                        {
                            pointsOfInterest[new Point(col, row - 1)] = character + "" + down;
                        }
                        else if (char.IsLetter(right) && left == '.')
                        {
                            pointsOfInterest[new Point(col - 1, row)] = character + "" + right;
                        }
                        else if (char.IsLetter(left) && right == '.')
                        {
                            pointsOfInterest[new Point(col + 1, row)] = left + "" + character;
                        }
                    }
                }
            }
            return pointsOfInterest;
        }
    }
}
