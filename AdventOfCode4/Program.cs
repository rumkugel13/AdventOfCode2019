using System;
using System.Linq;

namespace AdventOfCode4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const int min = 271973;
            const int max = 785961;

            int count1 = 0, count2 = 0, count3 = 0;

            for (int i = min; i < max; i++)
            {
                int number = i;
                int[] digits = GetDigits(number);

                if (IsIncreasing(digits) && HasDouble(digits))
                {
                    count1++;
                }

                if (IsIncreasing(digits) && ContainsExplicitDouble(digits))
                {
                    count2++;
                }

                if (IsIncreasing(digits) && ContainsExplicitDoubleNew(digits))
                {
                    count3++;
                }
            }

            Console.WriteLine($"Count: {count1} , {count2}, {count3}");
            Console.ReadKey();
        }

        static int[] GetDigits(int number)
        {
            int[] digits = new int[6];

            for (int j = digits.Length - 1; j >= 0; j--)
            {
                digits[j] = number % 10;
                number /= 10;
            }

            return digits;
        }

        static bool ContainsExplicitDoubleNew(int[] digits)
        {
            int[] count = new int[10];

            for (int i = 0; i < digits.Length; i++)
            {
                count[digits[i]]++; //count digits, have to be in ascending order, so doubles can only be adjacent
            }

            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] == 2)
                {
                    return true;
                }
            }

            return false;
        }

        static bool ContainsExplicitDouble(int[] digits)
        {
            int[] doubles = new int[10];

            for (int i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i] == digits[i + 1])
                {
                    if (doubles[digits[i]] == 0)
                    {
                        doubles[digits[i]] = 1; //first time double
                    }
                    else
                    {
                        doubles[digits[i]] = 2; //too many
                    }
                }
            }

            for (int i = 0; i < doubles.Length; i++)
            {
                if (doubles[i] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsIncreasing(int[] digits)
        {
            for (int i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i] > digits[i + 1])
                {
                    return false;
                }
            }

            return true;
        }

        static bool HasDouble(int[] digits)
        {
            for (int i = 0; i < digits.Length - 1; i++)
            {
                if (digits[i] == digits[i+1])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
