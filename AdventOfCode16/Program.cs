using System;
using System.IO;

namespace AdventOfCode16
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int[] input_array;

            using (StreamReader sr = new StreamReader(File.OpenRead("data.txt")))
            {
                char[] input = sr.ReadToEnd().ToCharArray();
                input_array = new int[input.Length];
                for (int i = 0; i < input.Length; i++)
                {
                    input_array[i] = Convert.ToInt32(input[i].ToString());
                }
            }

            //input_array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };//test data 0

            Console.WriteLine($"Number of digits: {input_array.Length}");

            int[] base_pattern = { 0, 1, 0, -1 };

            // Part 1 | Result: 69549155
            
            int[] temp_output;
            int[] last_output = (int[])input_array.Clone();

            for (int phase = 0; phase < 100; phase++)
            {
                temp_output = new int[last_output.Length];

                for (int j = 0; j < last_output.Length; j++)  //current output index
                {
                    int current_output = 0;

                    for (int k = 0; k < last_output.Length; k++)  //curent input index
                    {
                        current_output += last_output[k] * base_pattern[(k + 1) / (j + 1) % 4];
                    }

                    temp_output[j] = Math.Abs(current_output) % 10;    //only save last digit
                }

                last_output = (int[])temp_output.Clone();
            }

            Console.Write("Result1: ");
            WriteArray(last_output, 8, 0);

            // Part 2 | Result: 83253465

            int offset = 0;
            for (int i = 6, mod = 1; i >= 0; i--, mod *= 10)
            {
                offset += mod * input_array[i];
            }

            Console.WriteLine($"Offset: {offset}");

            int[] large_input = new int[10000 * input_array.Length];
            for (int i = 0; i < 10000; i++)
            {
                Array.Copy(input_array, 0, large_input, i * input_array.Length, input_array.Length);
            }

            Console.WriteLine($"Created large input");

            // you can split the number of multiplications in half by observing that all the numbers 1 step below the diagonal are always multiplied by zero
            // according to the sequence, after half of the input, the second half consists only of *1, while the first half consists of *0
            // note: the last n output digits only consist of the sum of the last n input digits (this is true for the second half of the output)
            //       since offset appears to be in second half, we can ignore the other calculations (even the sequence)

            last_output = (int[])large_input.Clone();
            int[] temp_large = new int[last_output.Length];
            for (int i = 0; i < 100; i++)
            {
                int sum = 0;
                for (int it = last_output.Length - 1; it >= last_output.Length / 2; it--)   //only calculate the digits for the output
                {
                    sum += last_output[it]; //some pattern suggests this is all thats needed | edit: see above
                    temp_large[it] = sum % 10;
                }
                last_output = temp_large;
            }

            Console.Write("Result2 at 0: ");    //note: first part is not being calculated
            WriteArray(last_output, 8, 0);

            Console.Write("Result2 at offset: ");
            WriteArray(last_output, 8, offset);

            Console.ReadKey();
        }

        static void WriteArray(int[] data, int count, int offset)
        {
            for (int i = offset; i < data.Length && i < offset + count; i++)
            {
                Console.Write(data[i]);
            }
            Console.Write("\n");
        }
    }
}
