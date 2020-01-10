using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode23
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Dictionary<long, long> memory = new Dictionary<long, long>();

            using (StreamReader reader = new StreamReader(File.OpenRead("data.txt")))
            {
                var data = reader.ReadToEnd().Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    memory[i] = Convert.ToInt64(data[i]);
                }
            }

            Computer[] computers = new Computer[50];

            for (int i = 0; i < computers.Length; i++)
            {
                computers[i] = new Computer(memory);
                computers[i].Input.Enqueue(i);
            }

            List<Packet> packetQueue = new List<Packet>();
            Packet NATpacket = new Packet();
            bool[] isIdle = new bool[50];
            long lastYSent = 0;

            while (true)
            {
                for (int i = 0; i < computers.Length; i++)
                {
                    Computer computer = (Computer)computers[i];

                    if (packetQueue.Where(p => p.ID == i).Count() > 0)
                    {
                        Packet pack = packetQueue.Where(p => p.ID == i).First();
                        computer.Input.Enqueue(pack.X);
                        computer.Input.Enqueue(pack.Y);
                        packetQueue.Remove(pack);
                    }

                    Computer.ReturnCodes returnCode = computer.Compute();

                    if (returnCode == Computer.ReturnCodes.HASOUTPUT)
                    {
                        Packet packet;
                        packet.ID = (int)computer.Output.Dequeue();
                        computer.Compute();
                        packet.X = computer.Output.Dequeue();
                        computer.Compute();
                        packet.Y = computer.Output.Dequeue();

                        if (packet.ID == 255)
                        {
                            Console.WriteLine($"Part1 Found: {packet.ID} {packet.X} {packet.Y}"); //Part1: 21089
                            NATpacket = packet;
                        }
                        else
                        {
                            packetQueue.Add(packet);
                        }
                        isIdle[i] = false;
                    }

                    if (returnCode == Computer.ReturnCodes.NEEDINPUT)
                    {
                        computer.Input.Enqueue(-1);
                        isIdle[i] = true;
                    }
                }

                // handle NAT
                if (isIdle.Where(b => b == true).Count() == 50)
                {
                    Packet temp = NATpacket;
                    temp.ID = 0;
                    packetQueue.Add(temp);
                    Console.WriteLine($"NAT sent: {temp.Y}");

                    if (temp.Y == lastYSent && lastYSent != 0)
                    {
                        Console.WriteLine($"Part2 Found: {lastYSent}"); //Part2: 16658
                        break;
                    }
                    else
                    {
                        lastYSent = temp.Y;
                    }
                }
            }

            Console.ReadKey();
        }

        struct Packet
        {
            public int ID;
            public long X;
            public long Y;
        }
    }
}
