using System;
using System.IO;

namespace AdventOfCode5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int result1 = 0, result2 = 0;
            int[] input;

            using (StreamReader reader = new StreamReader(File.OpenRead("test2.txt")))
            {
                var data = reader.ReadToEnd().Split(',');
                input = new int[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    string s = data[i];
                    input[i] = Convert.ToInt32(s);
                }
            }

            //

            int[] fresh_input = new int[input.Length];
            Array.Copy(input, fresh_input, input.Length);

            result1 = Compute(1, fresh_input);

            Array.Copy(input, fresh_input, input.Length);

            result2 = Compute(5, fresh_input);

            //

            Console.WriteLine($"Result: {result1} {result2}");

            // results: 5044655 7408802

            Console.ReadKey();
        }

        static int[] GetDigits(int number)
        {
            int[] digits = new int[5];

            for (int j = digits.Length - 1; j >= 0; j--)
            {
                digits[j] = number % 10;
                number /= 10;
            }

            return digits;
        }

        private static int Compute(int input, int[] data)
        {
            Console.WriteLine("Running with input " + input);

            int IC = 0; // instruction counter
            int output = 0;

            while (IC < data.Length)
            {
                int opcode = data[IC];

                int[] mode = GetDigits(opcode); // deconstruct opcode into EDCBA

                int instruction = 10 * mode[3] + mode[4];   // instruction is BA of opcode

                int mode3 = mode[0];    //mode for parameter 3 is E
                int mode2 = mode[1];    //mode for parameter 2 is D
                int mode1 = mode[2];    //mode for parameter 1 is C

                int param1 = IC + 1 < data.Length ? data[IC + 1] : 0;   // if parameter available, store in variable
                int param2 = IC + 2 < data.Length ? data[IC + 2] : 0;
                int param3 = IC + 3 < data.Length ? data[IC + 3] : 0;

                int value1 = param1 < data.Length ? (mode1 == 0 ? data[param1] : param1) : 0;   // if position available, check mode and store in variable
                int value2 = param2 < data.Length ? (mode2 == 0 ? data[param2] : param2) : 0;   // 0:position mode, 1:immediate mode
                int value3 = param3 < data.Length ? (mode3 == 0 ? data[param3] : param3) : 0;

                switch (instruction)
                {
                    case 1: //add first two parameters, store in last | 3 params
                        {
                            data[param3] = value1 + value2;
                            IC += 4;
                            break;
                        }
                    case 2: //multiply first two parameters, store in last | 3 params
                        {
                            data[param3] = value1 * value2;
                            IC += 4;
                            break;
                        }
                    case 3: //take input, and store it at address in instruction | 1 param
                        {
                            data[param1] = input;
                            IC += 2;
                            break;
                        }
                    case 4: //output from address in instruction | 1 param
                        {
                            output = value1;
                            Console.WriteLine("Output: " + output);
                            IC += 2;
                            break;
                        }
                    case 5: //jump-if-true | 2 params
                        {
                            IC = value1 != 0 ? value2 : IC + 3;
                            break;
                        }
                    case 6: //jump-if-false | 2 params
                        {
                            IC = value1 == 0 ? value2 : IC + 3;
                            break;
                        }
                    case 7: //less than | 3 params
                        {
                            data[param3] = value1 < value2 ? 1 : 0;
                            IC += 4;
                            break;
                        }
                    case 8: //equals | 3 params
                        {
                            data[param3] = value1 == value2 ? 1 : 0;
                            IC += 4;
                            break;
                        }
                    case 99://HALT | 0 params
                        IC = data.Length;
                        break;
                }
            }

            return output;
        }
    }
}
