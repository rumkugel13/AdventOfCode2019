using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode21
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string input = "NOT A J\nNOT B T\nOR T J\nNOT C T\nOR T J\nAND D J\nWALK";
            Run(input);

            input = "OR A T\nAND B T\nAND C T\nNOT T J\nAND D J\nNOT H T\nNOT T T\nOR E T\nAND T J\nRUN";
            Run(input);

            Console.ReadKey();
        }

        static void Run(string input)
        {
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

                    if (output > char.MaxValue)
                        Console.WriteLine(output);

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
                    //if (input == "")
                    //    input = Console.ReadLine();

                    foreach (char c in input)
                    {
                        computer.Input.Enqueue(c);
                    }
                    computer.Input.Enqueue(10);
                }

            }
            while (returnCode != Computer.ReturnCodes.HALT);
        }
    }
}
