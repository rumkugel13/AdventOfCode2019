using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode17
{
    class Program
    {
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

            Dictionary<long, long> copy = new Dictionary<long, long>(memory);   //for part2

            Computer computer = new Computer(memory);
            Computer.ReturnCodes returnCode;
            Point position = new Point(0, 0);
            Point robot = new Point(0);

            Dictionary<Point, long> items = new Dictionary<Point, long>();

            do
            {
                returnCode = computer.Compute();

                if (returnCode == Computer.ReturnCodes.HASOUTPUT)
                {
                    long output = computer.Output.Dequeue();

                    if (output == 10)   //new line
                    {
                        position.X = 0;
                        position.Y++;
                    }
                    else
                    {
                        if (output != '#' && output != '.')
                        {
                            robot = new Point(position.X, position.Y);
                        }

                        items[position] = output;
                        position.X++;
                    }
                }

                if (returnCode == Computer.ReturnCodes.NEEDINPUT)
                {
                    Console.SetCursorPosition(0, 0);
                    PrintScreen(items);
                    Console.WriteLine($"Robot: {robot}");
                }

            }
            while (returnCode != Computer.ReturnCodes.HALT);

            Console.SetCursorPosition(0, 0);
            PrintScreen(items);
            Console.WriteLine($"Robot: {robot}");

            // part1 | result1: 14332

            List<Point> intersections = Intersections(items);

            int result1 = 0;

            foreach (Point p in intersections)
            {
                result1 += p.X * p.Y;
            }

            Console.WriteLine($"Result1: {result1}");

            // part2 | result2: 1034009

            /*
                R12L8R12 R8R6R6R8 R12L8R12 R8R6R6R8 R8L8R8R4R4 R8L8R8R4R4 R8R6R6R8 R8L8R8R4R4 R8R6R6R8 R12L8R12

                A: R12L8R12
                B: R8R6R6R8
                C: R8L8R8R4R4

                MAIN: ABABCCBCBA
            */

            copy[0] = 2;    //wake up robot
            Computer computer2 = new Computer(copy);

            char[] mainRoutine = "A,B,A,B,C,C,B,C,B,A\n".ToCharArray();//  { 'A', ',', 'B', ',', 'A', ',', 'B', ',', 'C', ',', 'C', ',', 'B', ',', 'C', ',', 'B', ',', 'A', (char)10 };
            char[] routineA = "R,12,L,8,R,12\n".ToCharArray();
            char[] routineB = "R,8,R,6,R,6,R,8\n".ToCharArray();
            char[] routineC = "R,8,L,8,R,8,R,4,R,4\n".ToCharArray();

            foreach (char c in mainRoutine)
            {
                computer2.Input.Enqueue(c);
            }

            foreach (char c in routineA)
            {
                computer2.Input.Enqueue(c);
            }

            foreach (char c in routineB)
            {
                computer2.Input.Enqueue(c);
            }

            foreach (char c in routineC)
            {
                computer2.Input.Enqueue(c);
            }

            computer2.Input.Enqueue('n');
            computer2.Input.Enqueue('\n');

            long result2 = 0;

            do
            {
                returnCode = computer2.Compute();

                if (returnCode == Computer.ReturnCodes.HASOUTPUT)
                {
                    long output = computer2.Output.Dequeue();

                    if (output >= 128)
                    {
                        result2 = output;
                    }
                }
            }
            while (returnCode != Computer.ReturnCodes.HALT);

            Console.WriteLine($"Result2: {result2}");

            Console.ReadKey();
        }

        static List<Point> Intersections(Dictionary<Point, long> items)
        {
            List<Point> list = new List<Point>();

            foreach (Point p in items.Keys)
            {
                if (items[p] != '#')
                    continue;

                Point[] possiblePoints = new Point[]
                {
                    new Point(p.X, p.Y - 1),
                    new Point(p.X, p.Y + 1),
                    new Point(p.X + 1, p.Y),
                    new Point(p.X - 1, p.Y)
                };

                bool isIntersection = true;
                foreach (Point possible in possiblePoints)
                {
                    if (items.TryGetValue(possible, out long value))
                    {
                        if (value != '#')
                        {
                            isIntersection = false;
                        }
                    }
                    else
                    {
                        isIntersection = false;
                    }
                }

                if (isIntersection)
                {
                    list.Add(p);
                }
            }

            return list;
        }

        static void PrintScreen(Dictionary<Point, long> items)
        {
            MinMax(items, out Point min, out Point max);

            int width = max.X - min.X + 1;
            int height = max.Y - min.Y + 1;
            int offX = min.X, offY = min.Y;

            char[][] characters = new char[height][];

            for (int i = 0; i < height; i++)
            {
                characters[i] = new char[width];

                for (int j = 0; j < width; j++)
                {
                    if (items.TryGetValue(new Point(j - offX, i - offY), out long cha))
                    {
                        characters[i][j] = (char)cha;
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                string buffer = "";
                for (int j = 0; j < width; j++)
                {
                    buffer += characters[i][j];
                }
                Console.WriteLine(buffer);
            }
        }

        static void MinMax(Dictionary<Point, long> items, out Point min, out Point max)
        {
            min = new Point();
            min.X = items.Min(x => x.Key.X);
            min.Y = items.Min(y => y.Key.Y);

            max = new Point();
            max.X = items.Max(x => x.Key.X);
            max.Y = items.Max(y => y.Key.Y);
        }
    }
}
