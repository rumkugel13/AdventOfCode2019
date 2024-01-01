using System;
using System.IO;

namespace AdventOfCode2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int[] input;

            using (StreamReader reader = new StreamReader(File.OpenRead("data.txt")))
            {
                var data = reader.ReadToEnd().Split(',');
                input = new int[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    string s = data[i];
                    input[i] = Convert.ToInt32(s);
                }
            }

            for (int i = 0; i < 99; i++)
            {
                for (int j = 0; j < 99; j++)
                {
                    int[] currentrun = new int[input.Length];
                    Array.Copy(input, currentrun, input.Length);
                    int result = Function(i, j, currentrun);
                    if (result == 19690720)
                    {
                        Console.WriteLine("Result: " + result + " " + i + " " + j + "result: " + (100 * i + j));
                    }
                }
            }


            //int result = Function(12, 2, currentrun);

            //Console.WriteLine("Value: " + result);

            Console.ReadKey();
        }

        private static int Function(int verb, int noun, int[] copy)
        {
            copy[1] = verb;
            copy[2] = noun;

            int IC = 0;
            int instruction = copy[IC];

            while (IC < copy.Length)
            {
                instruction = copy[IC];
                int data1 = copy[IC + 1];
                int data2 = copy[IC + 2];
                int store = copy[IC + 3];

                switch (instruction)
                {
                    case 1:
                        copy[store] = copy[data1] + copy[data2];
                        break;
                    case 2:
                        copy[store] = copy[data1] * copy[data2];
                        break;
                    case 99:
                        IC = copy.Length;
                        break;
                }

                IC += 4;
            }

            return copy[0];
        }
    }
}
