using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode15
{
    class Computer
    {
        public long IC { get; private set; } = 0;

        public Dictionary<long, long> Memory { get; private set; }

        public Queue<long> Input { get; private set; }

        public Queue<long> Output { get; private set; }

        private long relativeBase = 0;

        public Computer(Dictionary<long, long> instructions)
        {
            this.Memory = new Dictionary<long, long>(instructions);
            this.Input = new Queue<long>();
            this.Output = new Queue<long>();
        }

        public ReturnCodes Compute()
        {
            while (this.IC < this.Memory.Count)
            {
                long opcode = this.Memory[this.IC];

                long[] mode = GetDigits(opcode); // deconstruct opcode into EDCBA

                long instruction = 10 * mode[3] + mode[4];   // instruction is BA of opcode

                long mode3 = mode[0];    //mode for parameter 3 is E
                long mode2 = mode[1];    //mode for parameter 2 is D
                long mode1 = mode[2];    //mode for parameter 1 is C

                long param1 = GetParameter(1); // if parameter available, store in variable
                long param2 = GetParameter(2);
                long param3 = GetParameter(3);

                long value1 = GetValue(param1, mode1);
                long value2 = GetValue(param2, mode2);
                long value3 = GetValue(param3, mode3);  // note: param3 (output) never in immediate mode

                switch ((Instructions)instruction)
                {
                    case Instructions.Add: //3 params
                        {
                            SetValue(param3, mode3, value1 + value2);
                            this.IC += 4;
                            break;
                        }
                    case Instructions.Multiply: //3 params
                        {
                            SetValue(param3, mode3, value1 * value2);
                            this.IC += 4;
                            break;
                        }
                    case Instructions.Input: //1 param
                        {
                            if (this.Input.Count == 0)
                            {
                                return ReturnCodes.NEEDINPUT;   //wait for input
                            }

                            SetValue(param1, mode1, this.Input.Dequeue());
                            this.IC += 2;
                            break;
                        }
                    case Instructions.Output: //1 param
                        {
                            this.Output.Enqueue(value1);
                            //Console.WriteLine($"Computer {this.ID} Output: {value1}");
                            this.IC += 2;
                            return ReturnCodes.HASOUTPUT;
                            break;
                        }
                    case Instructions.JumpIfTrue: //2 params
                        {
                            this.IC = value1 != 0 ? value2 : IC + 3;
                            break;
                        }
                    case Instructions.JumpIfFalse: //2 params
                        {
                            this.IC = value1 == 0 ? value2 : IC + 3;
                            break;
                        }
                    case Instructions.LessThan: //3 params
                        {
                            SetValue(param3, mode3, value1 < value2 ? 1 : 0);
                            this.IC += 4;
                            break;
                        }
                    case Instructions.Equals: //3 params
                        {
                            SetValue(param3, mode3, value1 == value2 ? 1 : 0);
                            this.IC += 4;
                            break;
                        }
                    case Instructions.AdjustRelativeBase: //1 param
                        {
                            this.relativeBase += value1;
                            this.IC += 2;
                            break;
                        }
                    case Instructions.Halt://0 params
                        this.IC = 0;
                        return ReturnCodes.HALT;
                }
            }

            return ReturnCodes.ERROR;
        }

        private long[] GetDigits(long number)
        {
            long[] digits = new long[5];

            for (int j = digits.Length - 1; j >= 0; j--)
            {
                digits[j] = number % 10;
                number /= 10;
            }

            return digits;
        }

        public enum ReturnCodes
        {
            HALT, NEEDINPUT, HASOUTPUT, ERROR
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

        private long GetParameter(long index)
        {
            if (this.Memory.TryGetValue(this.IC + index, out long value))
            {
                return value;
            }

            return 0;
        }

        private void SetValue(long parameter, long mode, long value)
        {
            switch ((Modes)mode)
            {
                case Modes.Position:
                    {
                        this.Memory[parameter] = value;
                        break;
                    }
                case Modes.Immediate:
                    throw new ArgumentException("Immediate mode not supported");
                case Modes.Relative:
                    {
                        this.Memory[this.relativeBase + parameter] = value;
                        break;
                    }
            }
        }

        private long GetValue(long parameter, long mode)
        {
            switch ((Modes)mode)
            {
                case Modes.Position:
                    {
                        if (this.Memory.TryGetValue(parameter, out long value))
                        {
                            return value;
                        }
                        break;
                    }
                case Modes.Immediate:
                    return parameter;
                case Modes.Relative:
                    {
                        if (this.Memory.TryGetValue(this.relativeBase + parameter, out long value))
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
