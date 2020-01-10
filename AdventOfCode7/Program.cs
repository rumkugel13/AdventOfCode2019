using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int result1 = 0, result2 = 0;
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

            //


            {
                var sequences = GetSequences(0, 5);

                int largest_output = 0;

                foreach (int[] sequence in sequences)
                {
                    int last_output = 0;

                    for (int j = 0; j < 5; j++) //run each amplifier once
                    {
                        Queue<int> queue = new Queue<int>();
                        queue.Enqueue(sequence[j]);               // phase setting
                        queue.Enqueue(last_output);     // output from last
                        int[] progMemory = (int[])input.Clone();
                        int ic = 0;
                        Compute(ref progMemory, ref ic, input: queue, output: out int output);
                        last_output = output;
                    }

                    if (last_output > largest_output)
                    {
                        largest_output = last_output;
                    }
                }

                result1 = largest_output;
            }

            //

            {
                var sequences = GetSequences(5, 10);

                int largest_output = 0;

                foreach (int[] sequence in sequences)
                {
                    bool first = true;

                    int[][] programdata = new int[5][];
                    int[] programIC = new int[5];
                    bool end = false;
                    int last_output = 0;

                    //Console.WriteLine($"Current sequence: [{sequence[0]},{sequence[1]},{sequence[2]},{sequence[3]},{sequence[4]}]");

                    while (!end)
                    {
                        for (int i = 0; i < 5; i++) //run each amplifier once
                        {
                            ReturnCodes returncode = 0;
                            Queue<int> inputQueue = new Queue<int>();
                            int output;
                            int ic = first ? 0 : programIC[i];
                            int[] progdata = first ? (int[])input.Clone() : programdata[i];

                            if (first)
                            {
                                inputQueue.Enqueue(sequence[i]);     // phase setting
                            }

                            inputQueue.Enqueue(last_output);     // output from last
                            returncode = Compute(ref progdata, ref ic, inputQueue, out output);

                            programIC[i] = ic;
                            programdata[i] = progdata;

                            last_output = output;

                            //Console.WriteLine($"Amplifier {i}: {last_output} {returncode}");

                            if (i == 4 && returncode != ReturnCodes.NEEDINPUT || last_output < 0)
                            {
                                end = true;
                            }

                        }

                        first = false;
                    }

                    if (last_output > largest_output)
                    {
                        largest_output = last_output;
                    }
                }

                result2 = largest_output;
            }

            //

            Console.WriteLine($"Result: {result1} {result2}");

            // results: 117312 1336480

            Console.ReadKey();
        }

        static List<int[]> GetSequences(int start, int end)
        {
            List<int[]> sequences = new List<int[]>();
            HashSet<int> taken = new HashSet<int>();

            for (int i = start; i < end; i++)
            {
                taken.Clear();
                taken.Add(i);
                for (int j = start; j < end; j++)
                {
                    if (!taken.Contains(j))
                    {
                        taken.Add(j);
                        for (int k = start; k < end; k++)
                        {
                            if (!taken.Contains(k))
                            {
                                taken.Add(k);
                                for (int l = start; l < end; l++)
                                {
                                    if (!taken.Contains(l))
                                    {
                                        taken.Add(l);
                                        for (int m = start; m < end; m++)
                                        {
                                            if (!taken.Contains(m))
                                            {
                                                int[] seq = new int[] { i, j, k, l, m };
                                                sequences.Add(seq);
                                            }
                                        }
                                        taken.Remove(l);
                                    }
                                }
                                taken.Remove(k);
                            }
                        }
                        taken.Remove(j);
                    }
                }
                taken.Remove(i);
            }

            return sequences;
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

        enum ReturnCodes
        {
            HALT, NEEDINPUT, ERROR
        }

        private static ReturnCodes Compute(ref int[] memory, ref int progIC, Queue<int> input, out int output)
        {
            output = 0;

            while (progIC < memory.Length)
            {
                int opcode = memory[progIC];

                int[] mode = GetDigits(opcode); // deconstruct opcode into EDCBA

                int instruction = 10 * mode[3] + mode[4];   // instruction is BA of opcode

                int mode3 = mode[0];    //mode for parameter 3 is E
                int mode2 = mode[1];    //mode for parameter 2 is D
                int mode1 = mode[2];    //mode for parameter 1 is C

                int param1 = progIC + 1 < memory.Length ? memory[progIC + 1] : 0;   // if parameter available, store in variable
                int param2 = progIC + 2 < memory.Length ? memory[progIC + 2] : 0;
                int param3 = progIC + 3 < memory.Length ? memory[progIC + 3] : 0;

                int value1 = param1 < memory.Length ? (mode1 == 0 ? memory[param1] : param1) : 0;   // if position available, check mode and store in variable
                int value2 = param2 < memory.Length ? (mode2 == 0 ? memory[param2] : param2) : 0;   // 0:position mode, 1:immediate mode
                int value3 = param3 < memory.Length ? (mode3 == 0 ? memory[param3] : param3) : 0;   // note: param3 (output) always in position mode

                switch (instruction)
                {
                    case 1: //add first two parameters, store in last | 3 params
                        {
                            memory[param3] = value1 + value2;
                            progIC += 4;
                            break;
                        }
                    case 2: //multiply first two parameters, store in last | 3 params
                        {
                            memory[param3] = value1 * value2;
                            progIC += 4;
                            break;
                        }
                    case 3: //take input, and store it at address in instruction | 1 param
                        {
                            if (input.Count == 0)
                            {
                                return ReturnCodes.NEEDINPUT;   //wait for input
                            }

                            memory[param1] = input.Dequeue();
                            progIC += 2;
                            break;
                        }
                    case 4: //output from address in instruction | 1 param
                        {
                            output = value1;
                            //Console.WriteLine("Output: " + output);
                            progIC += 2;
                            break;
                        }
                    case 5: //jump-if-true | 2 params
                        {
                            progIC = value1 != 0 ? value2 : progIC + 3;
                            break;
                        }
                    case 6: //jump-if-false | 2 params
                        {
                            progIC = value1 == 0 ? value2 : progIC + 3;
                            break;
                        }
                    case 7: //less than | 3 params
                        {
                            memory[param3] = value1 < value2 ? 1 : 0;
                            progIC += 4;
                            break;
                        }
                    case 8: //equals | 3 params
                        {
                            memory[param3] = value1 == value2 ? 1 : 0;
                            progIC += 4;
                            break;
                        }
                    case 99://HALT | 0 params
                        progIC = 0;
                        return ReturnCodes.HALT;
                }
            }

            return ReturnCodes.ERROR;
        }
    }
}
