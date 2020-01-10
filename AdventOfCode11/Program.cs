using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AdventOfCode11
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

            // part 1

            Computer computer = new Computer(memory);
            Computer.ReturnCodes returnCode;
            Direction currentDirection = Direction.Up;
            long currentColor = 0;
            Point currentPosition = new Point(0, 0);
            Dictionary<Point, long> colors = new Dictionary<Point, long>();
            colors.Add(currentPosition, 1); //start with white for #2

            do
            {
                if (colors.TryGetValue(currentPosition, out long color))    //get color at current position
                {
                    currentColor = color;
                }
                else
                {
                    currentColor = 0;    //default black
                }

                computer.Input.Enqueue(currentColor); // input current color

                returnCode = computer.Compute();

                long nextColor = computer.Output.Dequeue(); // output new color

                colors[currentPosition] = nextColor;    // paint color at position

                Console.WriteLine($"In: {currentColor} Out: {nextColor} at {currentPosition.X},{currentPosition.Y} Visited: {colors.Count}");

                long direction = computer.Output.Dequeue(); // output new direction
                currentDirection = NextDirection(currentDirection, direction);

                currentPosition = MoveForward(currentDirection, currentPosition);
            }
            while (returnCode != Computer.ReturnCodes.HALT);

            Console.WriteLine($"Positions visited at least once: {colors.Count}");

            // result: 1681
            // part 2

            int height = 6, width = 41;
            int[][] pixels = new int[height][];
            const int pixelblack = 0, pixelwhite = 1;

            for (int i = 0; i < height; i++)
            {
                pixels[i] = new int[width];

                for (int j = 0; j < width; j++)
                {
                    if (colors.TryGetValue(new Point(j,i), out long color))
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
                    if (pixels[i][j] == pixelblack)
                        buffer += ' ';
                    if (pixels[i][j] == pixelwhite)
                        buffer += '█';
                }
                Console.WriteLine(buffer);
            }

            // result: EGZCRKGK

            Console.ReadKey();
        }

        static Point MoveForward(Direction direction, Point position)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Point(position.X, position.Y - 1);
                case Direction.Down:
                    return new Point(position.X, position.Y + 1);
                case Direction.Left:
                    return new Point(position.X - 1, position.Y);
                case Direction.Right:
                    return new Point(position.X + 1, position.Y);
            }

            return position;
        }

        static Direction NextDirection(Direction current, long input)
        {
            // input 0: left 90deg, 1: right 90deg
            switch (current)
            {
                case Direction.Up:
                    return input == 0 ? Direction.Left : Direction.Right;
                case Direction.Down:
                    return input == 0 ? Direction.Right : Direction.Left;
                case Direction.Left:
                    return input == 0 ? Direction.Down : Direction.Up;
                case Direction.Right:
                    return input == 0 ? Direction.Up : Direction.Down;
            }

            return current;
        }

        enum Direction
        {
            Up, Down, Left, Right
        }
    }
}
