using System;
using System.IO;

namespace AdventOfCode1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            long sum = 0;

            using (StreamReader streamReader = new StreamReader(File.OpenRead("data.txt")))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    long value = Convert.ToInt64(line);
                    long fuel = CalculateFuel(value);
                    Console.WriteLine("fuel " + fuel);
                    sum += fuel;

                    while (fuel > 0)
                    {
                        fuel = CalculateFuel(fuel);
                        if (fuel > 0)
                            sum += fuel;
                        Console.WriteLine("fuel " + fuel);
                    }
                }
            }

            Console.WriteLine("sum: " + sum);
            Console.ReadKey();
        }

        private static long CalculateFuel(long mass)
        {
            long value = mass;
            value /= 3;
            value -= 2;
            return value;
        }
    }
}
