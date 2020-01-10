using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode15
{
    class Program
    {
        static Computer computer;
        static Dictionary<Point, long> mapped;
        static Point start, target;
        static Dictionary<Point, long> floodLevels;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Dictionary<long, long> memory = new Dictionary<long, long>();

            using (StreamReader reader = new StreamReader(File.OpenRead("data.txt")))
            {
                var data = reader.ReadToEnd().Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    memory[i] = Convert.ToInt64(data[i]);
                }
            }

            computer = new Computer(memory);
            mapped = new Dictionary<Point, long>();

            start = new Point(20, 20);

            Console.WriteLine($"Start at: {start}");

            Computer.ReturnCodes code = computer.Compute();

            Walk(start);    //build map here

            Console.WriteLine($"Target found at: {target}");

            // part1 | result: 208

            floodLevels = new Dictionary<Point, long>();
            floodLevels[start] = 0;

            FloodFill(start);

            long result1 = floodLevels[target];

            Console.WriteLine($"Result: {result1}");

            // part2 | result: 306

            floodLevels = new Dictionary<Point, long>();
            floodLevels[target] = 0;

            FloodFill(target);

            long result2 = floodLevels.Values.Max();

            Console.WriteLine($"Result: {result2}");

            Console.ReadKey();
        }

        static void FloodFill(Point start)
        {
            Stack<Point> toProcess = new Stack<Point>();

            toProcess.Push(start);

            while (toProcess.Count > 0)
            {
                Point next = toProcess.Pop();

                Point[] possiblePoints = new Point[]
                {
                    new Point(next.X, next.Y - 1),
                    new Point(next.X, next.Y + 1),
                    new Point(next.X + 1, next.Y),
                    new Point(next.X - 1, next.Y)
                };

                foreach (Point p in possiblePoints)
                {
                    if (mapped.ContainsKey(p) && mapped[p] != (long)Result.Wall)
                    {
                        if (!floodLevels.ContainsKey(p))
                        {
                            floodLevels[p] = floodLevels[next] + 1;
                            toProcess.Push(p);
                        }
                    }
                }
            }
        }

        static void Walk(Point start)
        {
            //Point[] possiblePoints = new Point[]
            //{
            //    new Point(start.X, start.Y - 1),
            //    new Point(start.X, start.Y + 1),
            //    new Point(start.X + 1, start.Y),
            //    new Point(start.X - 1, start.Y)
            //};

            Point north = new Point(start.X, start.Y - 1);
            Point east = new Point(start.X + 1, start.Y);
            Point south = new Point(start.X, start.Y + 1);
            Point west = new Point(start.X - 1, start.Y);

            if (!mapped.ContainsKey(north))
            {
                Result result = MoveOneStep(Movement.North);
                mapped.Add(north, (long)result);

                if (result != Result.Wall)
                {
                    Walk(north);
                    // undo the move / backtrack
                    MoveOneStep(Movement.South);
                    if (result == Result.Target)
                    {
                        mapped[north] = (long)Result.Target;
                        target = new Point(north.X, north.Y);
                    }
                }
            }

            if (!mapped.ContainsKey(east))
            {
                Result result = MoveOneStep(Movement.East);
                mapped.Add(east, (long)result);

                if (result != Result.Wall)
                {
                    Walk(east);
                    // undo the move
                    MoveOneStep(Movement.West);
                    if (result == Result.Target)
                    {
                        mapped[east] = (long)Result.Target;
                        target = new Point(east.X, east.Y);
                    }
                }
            }

            if (!mapped.ContainsKey(south))
            {
                Result result = MoveOneStep(Movement.South);
                mapped.Add(south, (long)result);

                if (result != Result.Wall)
                {
                    Walk(south);
                    // undo the move
                    MoveOneStep(Movement.North);
                    if (result == Result.Target)
                    {
                        mapped[south] = (long)Result.Target;
                        target = new Point(south.X, south.Y);
                    }
                }
            }

            if (!mapped.ContainsKey(west))
            {
                Result result = MoveOneStep(Movement.West);
                mapped.Add(west, (long)result);

                if (result != Result.Wall)
                {
                    Walk(west);
                    // undo the move
                    MoveOneStep(Movement.East);
                    if (result == Result.Target)
                    {
                        mapped[west] = (long)Result.Target;
                        target = new Point(west.X, west.Y);
                    }
                }
            }
        }

        static Result MoveOneStep(Movement movement)
        {
            computer.Input.Enqueue((long)movement);
            computer.Compute();
            return (Result)computer.Output.Dequeue();
        }

        static void PrintScreen(Dictionary<Point, long> items)
        {
            int height = 40, width = 40;
            int[][] pixels = new int[height][];

            for (int i = 0; i < height; i++)
            {
                pixels[i] = new int[width];

                for (int j = 0; j < width; j++)
                {
                    if (items.TryGetValue(new Point(j, i), out long color))
                    {
                        pixels[i][j] = (int)color;
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                string buffer = "";
                for (int j = 0; j < width; j++)
                {
                    switch ((Items)pixels[i][j])
                    {
                        case Items.Unknown: buffer += ' '; break;
                        case Items.Wall: buffer += '█'; break;
                        case Items.Ship: buffer += 'O'; break;
                        case Items.System: buffer += 'S'; break;
                        case Items.Empty: buffer += '.'; break;
                    }
                }
                Console.WriteLine(buffer);
            }
        }

        enum Movement
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        enum Result
        {
            Wall = 0,
            Walkable = 1,
            Target = 2
        }

        enum Items
        {
            Unknown = 0,
            Wall = 1,
            Ship = 2,
            System = 3,
            Empty = 4,
        }
    }
}
