using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode19
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

            Computer computer = new Computer(memory);

            // Part1 | Result: 116

            long count_pulled = 0;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    computer = new Computer(memory);
                    computer.Input.Enqueue(i);
                    computer.Input.Enqueue(j);
                    computer.Compute();
                    count_pulled += computer.Output.Dequeue();
                }
            }

            Console.WriteLine($"Result1: {count_pulled}");

            // Part2 | Result: 

            Point position = new Point(0, 0);
            bool found = false;
            int currentX = 0;

            for (int y = 100; !found ; y++) //dont start too soon, there are gaps
            {
                //move y down; move from current x forward, check opposite position

                for (; !found; currentX++)
                {
                    computer = new Computer(memory);
                    computer.Input.Enqueue(currentX);
                    computer.Input.Enqueue(y);
                    computer.Compute();
                    if (computer.Output.Dequeue() == 1)
                        break;
                }

                // currentX and y is left most pulled from tractorbeam for current y

                computer = new Computer(memory);
                computer.Input.Enqueue(currentX + 99);
                computer.Input.Enqueue(y - 99);
                computer.Compute();
                if (computer.Output.Dequeue() == 1)
                {
                    position = new Point(currentX, y - 99);
                    found = true;
                }
            }

            Console.WriteLine($"Result2: {position.X * 10000 + position.Y}");

            Console.ReadKey();
        }
    }
}
