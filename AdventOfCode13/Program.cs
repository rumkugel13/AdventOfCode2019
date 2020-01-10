using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode13
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

            memory[0] = 2;
            Computer computer = new Computer(memory);
            Computer.ReturnCodes returnCode;

            Point outputPosition = new Point(0, 0);
            Point paddlePosition = new Point(0, 0);
            Point ballPosition = new Point(0, 0);
            Dictionary<Point, long> items = new Dictionary<Point, long>();
            int outputsRead = 0;
            long score = 0;

            do
            {
                returnCode = computer.Compute();

                if (returnCode == Computer.ReturnCodes.HASOUTPUT)
                {
                    if (outputsRead == 0)
                    {
                        outputPosition.X = (int)computer.Output.Dequeue();
                    }
                    else if (outputsRead == 1)
                    {
                        outputPosition.Y = (int)computer.Output.Dequeue();
                    }
                    else
                    {
                        if (outputPosition.X == -1 && outputPosition.Y == 0)
                        {
                            score = computer.Output.Dequeue();
                        }
                        else
                        {
                            Items item = (Items)computer.Output.Dequeue();

                            if (item == Items.Ball)
                            {
                                ballPosition = outputPosition;
                            }
                            else if(item == Items.HorizontalPaddle)
                            {
                                paddlePosition = outputPosition;
                            }

                            items[outputPosition] = (long)item;
                        }
                    }
                    outputsRead = (outputsRead + 1) % 3;
                }

                if (returnCode == Computer.ReturnCodes.NEEDINPUT)
                {
                    //Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    PrintScreen(items);
                    Console.WriteLine($"Score: {score}");

                    //if (ballPosition.X < paddlePosition.X)
                    //{
                    //    computer.Input.Enqueue(-1);  //move left
                    //}
                    //else if (ballPosition.X > paddlePosition.X)
                    //{
                    //    computer.Input.Enqueue(1);  //move right
                    //}
                    //else
                    //{
                    //    computer.Input.Enqueue(0);  //do nothing
                    //}

                    //System.Threading.Thread.Sleep(1);

                    var k = Console.ReadKey();
                    if (k.Key == ConsoleKey.LeftArrow)
                    {
                        computer.Input.Enqueue(-1);  //move left
                    }
                    else if (k.Key == ConsoleKey.RightArrow)
                    {
                        computer.Input.Enqueue(1);  //move right
                    }
                    else
                    {
                        computer.Input.Enqueue(0);  //do nothing
                    }
                }
            }
            while (returnCode != Computer.ReturnCodes.HALT);

            int blockTiles = items.Values.Count(x => x == (long)Items.Block);

            Console.WriteLine($"Blocknumber: {blockTiles}");    // result: 306 for part1, with only output
            Console.WriteLine($"Score: {score}");

            Console.ReadKey();
        }

        static void PrintScreen(Dictionary<Point, long> items)
        {
            MinMax(items, out Point min, out Point max);

            int width = max.X - min.X + 1;
            int height = max.Y - min.Y + 1;
            int offX = min.X, offY = min.Y;

            int[][] pixels = new int[height][];

            for (int i = 0; i < height; i++)
            {
                pixels[i] = new int[width];

                for (int j = 0; j < width; j++)
                {
                    if (items.TryGetValue(new Point(j - offX, i - offY), out long color))
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
                        case Items.Empty: buffer += ' '; break;
                        case Items.Wall: buffer += '█'; break;
                        case Items.Block: buffer += '#'; break;
                        case Items.HorizontalPaddle: buffer += '_'; break;
                        case Items.Ball: buffer += 'O'; break;
                    }
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

        enum Items
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            HorizontalPaddle = 3,
            Ball = 4
        }
    }
}
