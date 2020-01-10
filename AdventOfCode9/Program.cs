using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode9
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            long result1 = 0, result2 = 0;
            Dictionary<long, long> input = new Dictionary<long, long>();

            using (StreamReader reader = new StreamReader(File.OpenRead("data.txt")))
            {
                var data = reader.ReadToEnd().Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    string s = data[i];
                    input[i] = Convert.ToInt64(s);
                }
            }

            // Part 1

            Console.WriteLine($"Calculating part 1");

            Dictionary<long, long> memory = new Dictionary<long, long>(input);
            long IC = 0;
            Queue<long> queueInput = new Queue<long>();
            queueInput.Enqueue(1);
            Queue<long> queueOutput = new Queue<long>();
            Compute(memory, ref IC, queueInput, queueOutput);
            Console.WriteLine(string.Join(", ", queueOutput.ToArray()));

            // Part 2

            Console.WriteLine($"Calculating part 2");

            memory = new Dictionary<long, long>(input);
            IC = 0;
            queueInput = new Queue<long>();
            queueInput.Enqueue(2);
            queueOutput = new Queue<long>();
            Compute(memory, ref IC, queueInput, queueOutput);
            Console.WriteLine(string.Join(", ", queueOutput.ToArray()));

            // results: 4080871669, 75202

            Console.ReadKey();
        }

        private static ReturnCodes Compute(Dictionary<long, long> memory, ref long progIC, Queue<long> input, Queue<long> output)
        {
            long relative_base = 0;

            while (progIC < memory.Count)
            {
                long opcode = memory[progIC];

                long[] mode = GetDigits(opcode); // deconstruct opcode into EDCBA

                long instruction = 10 * mode[3] + mode[4];   // instruction is BA of opcode

                long mode3 = mode[0];    //mode for parameter 3 is E
                long mode2 = mode[1];    //mode for parameter 2 is D
                long mode1 = mode[2];    //mode for parameter 1 is C

                long param1 = GetParameter(1, progIC, memory); // if parameter available, store in variable
                long param2 = GetParameter(2, progIC, memory);
                long param3 = GetParameter(3, progIC, memory);

                long value1 = GetValue(param1, mode1, memory, relative_base);
                long value2 = GetValue(param2, mode2, memory, relative_base);
                long value3 = GetValue(param3, mode3, memory, relative_base);  // note: param3 (output) never in immediate mode

                switch ((Instructions)instruction)
                {
                    case Instructions.Add: //3 params
                        {
                            SetValue(param3, mode3, memory, relative_base, value1 + value2);
                            progIC += 4;
                            break;
                        }
                    case Instructions.Multiply: //3 params
                        {
                            SetValue(param3, mode3, memory, relative_base, value1 * value2);
                            progIC += 4;
                            break;
                        }
                    case Instructions.Input: //1 param
                        {
                            if (input.Count == 0)
                            {
                                return ReturnCodes.NEEDINPUT;   //wait for input
                            }

                            SetValue(param1, mode1, memory, relative_base, input.Dequeue());
                            progIC += 2;
                            break;
                        }
                    case Instructions.Output: //1 param
                        {
                            output.Enqueue(value1);
                            Console.WriteLine("Output: " + value1);
                            progIC += 2;
                            break;
                        }
                    case Instructions.JumpIfTrue: //2 params
                        {
                            progIC = value1 != 0 ? value2 : progIC + 3;
                            break;
                        }
                    case Instructions.JumpIfFalse: //2 params
                        {
                            progIC = value1 == 0 ? value2 : progIC + 3;
                            break;
                        }
                    case Instructions.LessThan: //3 params
                        {
                            SetValue(param3, mode3, memory, relative_base, value1 < value2 ? 1 : 0);
                            progIC += 4;
                            break;
                        }
                    case Instructions.Equals: //3 params
                        {
                            SetValue(param3, mode3, memory, relative_base, value1 == value2 ? 1 : 0);
                            progIC += 4;
                            break;
                        }
                    case Instructions.AdjustRelativeBase: //1 param
                        {
                            relative_base += value1;
                            progIC += 2;
                            break;
                        }
                    case Instructions.Halt://0 params
                        progIC = 0;
                        return ReturnCodes.HALT;
                }
            }

            return ReturnCodes.ERROR;
        }

        private static long[] GetDigits(long number)
        {
            long[] digits = new long[5];

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

        enum Modes
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

        enum Instructions
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            LessThan = 7,
            Equals = 8,
            AdjustRelativeBase = 9,
            Halt = 99
        }

        private static long GetParameter(long index, long IC, Dictionary<long, long> memory)
        {
            if (memory.TryGetValue(IC + index, out long value))
            {
                return value;
            }

            return 0;
        }

        private static void SetValue(long parameter, long mode, Dictionary<long, long> memory, long relative_base, long value)
        {
            switch ((Modes)mode)
            {
                case Modes.Position:
                    {
                        memory[parameter] = value;
                        break;
                    }
                case Modes.Immediate:
                    throw new ArgumentException("Immediate mode not supported");
                case Modes.Relative:
                    {
                        memory[relative_base + parameter] = value;
                        break;
                    }
            }
        }

        private static long GetValue(long parameter, long mode, Dictionary<long, long> memory, long relative_base)
        {
            switch ((Modes)mode)
            {
                case Modes.Position:
                    {
                        if (memory.TryGetValue(parameter, out long value))
                        {
                            return value;
                        }
                        break;
                    }
                case Modes.Immediate:
                    return parameter;
                case Modes.Relative:
                    {
                        if (memory.TryGetValue(relative_base + parameter, out long value))
                        {
                            return value;
                        }
                        break;
                    }
            }

            return 0;
        }
    }
}
