using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode25
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
            Computer.ReturnCodes returnCode;
            string buffer = "";

            do
            {
                returnCode = computer.Compute();

                if (returnCode == Computer.ReturnCodes.HASOUTPUT)
                {
                    long output = computer.Output.Dequeue();

                    if (output == 10)   //new line
                    {
                        Console.WriteLine(buffer);
                        buffer = "";
                    }
                    else
                    {
                        buffer += (char)output;
                    }
                }

                if (returnCode == Computer.ReturnCodes.NEEDINPUT)
                {
                    string input = Console.ReadLine();

                    foreach(char c in input)
                    {
                        computer.Input.Enqueue(c);
                    }
                    computer.Input.Enqueue(10);
                    //Console.SetCursorPosition(0, 0);
                    //PrintScreen(items);
                    //Console.WriteLine($"Robot: {robot}");
                }

            }
            while (returnCode != Computer.ReturnCodes.HALT);

            /*
             items:
                astronaut ice cream
                dark matter
                weather machine
                easter egg
             */
            // result: 285213704

            Console.ReadKey();
        }
    }
}
