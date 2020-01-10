using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode22
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Tuple<Instructions, int>> input = new List<Tuple<Instructions, int>>();

            using (StreamReader streamReader = new StreamReader(File.OpenRead("data.txt")))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();

                    if (line.StartsWith("cut"))
                    {
                        var value = Convert.ToInt32(line.Split(' ')[1]);
                        input.Add(new Tuple<Instructions, int>(Instructions.Cut, value));
                    }
                    else if (line.StartsWith("deal with increment"))
                    {
                        var value = Convert.ToInt32(line.Split(' ')[3]);
                        input.Add(new Tuple<Instructions, int>(Instructions.DealIncrement, value));
                    }
                    else if (line.StartsWith("deal into new stack"))
                    {
                        input.Add(new Tuple<Instructions, int>(Instructions.DealStack, 0));
                    }
                }
            }

            // Part1 | Result: 4096

            List<long> cards = new List<long>();

            for (long i = 0; i < 10007; i++)
            {
                cards.Add(i);
            }

            foreach (var t in input)
            {
                switch (t.Item1)
                {
                    case Instructions.DealIncrement:
                        DealIncrement(ref cards, t.Item2);
                        break;
                    case Instructions.Cut:
                        Cut(ref cards, t.Item2);
                        break;
                    case Instructions.DealStack:
                        DealStack(cards);
                        break;
                }
            }

            Console.WriteLine($"Result: {cards.IndexOf(2019)}");

            // Part2 | Result:

            //List<long> manycards = new List<long>();

            //for (long i = 0; i < 119315717514047; i++)
            //{
            //    manycards.Add(i);
            //}

            //for (long i = 0; i < 101741582076661; i++)
            //{
            //    foreach (var t in input)
            //    {
            //        switch (t.Item1)
            //        {
            //            case Instructions.DealIncrement:
            //                DealIncrement(ref manycards, t.Item2);
            //                break;
            //            case Instructions.Cut:
            //                Cut(ref manycards, t.Item2);
            //                break;
            //            case Instructions.DealStack:
            //                DealStack(manycards);
            //                break;
            //        }
            //    }
            //}

            //Console.WriteLine($"Result: {manycards[2020]}");

            Console.ReadKey();
        }

        static void DealIncrement(ref List<long> cards, int value)
        {
            long[] temp = new long[cards.Count];

            for (int i = 0; i < cards.Count; i++)
            {
                long number = cards[i];
                long index = (i * value) % cards.Count;
                temp[index] = number;
            }

            cards = new List<long>(temp);
        }

        static void Cut(ref List<long> cards, int value)
        {
            LinkedList<long> temp = new LinkedList<long>(cards);

            if (value > 0)  //rotate right
            {
                for (int i = 0; i < value; i++)
                {
                    var first = temp.First;
                    temp.RemoveFirst();
                    temp.AddLast(first);
                }
            }
            else if (value < 0)
            {
                for (int i = 0; i < Math.Abs(value); i++)
                {
                    var last = temp.Last;
                    temp.RemoveLast();
                    temp.AddFirst(last);
                }
            }

            cards = new List<long>(temp);
        }

        static void DealStack(List<long> cards)
        {
            cards.Reverse();
        }

        enum Instructions
        {
            DealIncrement,
            Cut,
            DealStack,
        }
    }
}
