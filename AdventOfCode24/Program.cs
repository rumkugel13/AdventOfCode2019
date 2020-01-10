using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode24
{
    class Program
    {
        static void Main(string[] args)
        {
            char[][] input = new char[5][];

            using (StreamReader streamReader = new StreamReader(File.OpenRead("data.txt")))
            {
                int i = 0;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    input[i++] = line.ToCharArray();
                }
            }

            // Part1 | Result: 1151290
            {
                char[][] data = (char[][])input.Clone();
                HashSet<int> seen = new HashSet<int>();

                while (true)
                {
                    if (!seen.Add(GetHash(data))) // second time
                    {
                        Console.WriteLine($"Result1: {Rating(data)}");
                        break;
                    }

                    char[][] temp = new char[5][];
                    for (int i = 0; i < data.Length; i++)
                    {
                        temp[i] = new char[5];
                        for (int j = 0; j < data[i].Length; j++)
                        {
                            int neighbours = Neighbours(data, i, j);

                            if (data[i][j] == '#')
                            {
                                temp[i][j] = neighbours == 1 ? '#' : '.';
                            }
                            else /*if (data[i][j] == '.')*/
                            {
                                temp[i][j] = neighbours == 1 || neighbours == 2 ? '#' : '.';
                            }
                        }
                    }

                    data = temp;
                }
            }
            // Part2 | Result: 1953

            bool[][] boolified = new bool[5][];
            for (int i = 0; i < 5; i++)
            {
                boolified[i] = new bool[5];
                for (int hj = 0; hj < 5; hj++)
                {
                    if (input[i][hj] == '#')
                    {
                        boolified[i][hj] = true;
                    }
                }
            }

            bool[][][] layers = new bool[15*15][][];
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new bool[5][];
                for (int j = 0; j < 5; j++)
                {
                    layers[i][j] = new bool[5];
                }
            }
            layers[15 * 15 / 2] = boolified;

            Console.WriteLine($"Initial Bugs: {CountBugs(layers)}");

            for (int minute = 0; minute < 200; minute++)
            {
                bool[][][] temp = new bool[layers.Length][][];

                for (int z = 0; z < layers.Length; z++)
                {
                    temp[z] = new bool[5][];
                    for (int y = 0; y < 5; y++)
                    {
                        temp[z][y] = new bool[5];
                        for (int x = 0; x < 5; x++)
                        {
                            if (x == 2 && y == 2) //ignore center piece
                            {
                                continue;
                            }

                            int neighbours = LayerNeighbours(layers, x, y, z);
                            bool isbug = layers[z][y][x];

                            if (isbug && neighbours == 1 || (!isbug && (neighbours == 1 || neighbours == 2)))
                            {
                                temp[z][y][x] = true;
                            }
                        }
                    }
                }
                //Console.WriteLine($"Minute: {minute + 1} Bugs: {CountBugs(temp)}");
                layers = temp;
            }

            long bugs = CountBugs(layers);

            Console.WriteLine($"Result2: {bugs}");
            //testdata : 1975
            Console.ReadKey();
        }

        static long CountBugs(bool[][][] layers)
        {
            long bugs = 0;

            for (int z = 0; z < layers.Length; z++)
            {
                for (int y = 0; y < layers[z].Length; y++)
                {
                    for (int x = 0; x < layers[z][y].Length; x++)
                    {
                        if (layers[z][y][x] == true)
                        {
                            bugs++;
                        }
                    }
                }
            }

            return bugs;
        }

        static int LayerNeighbours(bool[][][] layers, int x, int y, int z)
        {
            int neighbours = 0;

            //standard neighbours
            if (y - 1 >= 0 && layers[z][y - 1][x] == true) //up
            {
                neighbours++;
            }
            if (y + 1 < 5 && layers[z][y + 1][x] == true) //down
            {
                neighbours++;
            }
            if (x - 1 >= 0 && layers[z][y][x - 1] == true) //left
            {
                neighbours++;
            }
            if (x + 1 < 5 && layers[z][y][x + 1] == true) //right
            {
                neighbours++;
            }

            //special cases
            if (x == 0) //left side
            {
                if (z - 1 >= 0 && layers[z - 1][2][1] == true)
                {
                    neighbours++;
                }
            }

            if (x == 4) //right side
            {
                if (z - 1 >= 0 && layers[z - 1][2][3] == true)
                {
                    neighbours++;
                }
            }

            if (y == 0) //upper side
            {
                if (z - 1 >= 0 && layers[z - 1][1][2] == true)
                {
                    neighbours++;
                }
            }

            if (y == 4) //lower side
            {
                if (z - 1 >= 0 && layers[z - 1][3][2] == true)
                {
                    neighbours++;
                }
            }

            if (y == 1 && x == 2) //upper center
            {
                for (int l = 0; l < 5; l++)
                {
                    if (z + 1 < layers.Length && layers[z + 1][0][l] == true)
                    {
                        neighbours++;
                    }
                }
            }

            if (y == 3 && x == 2) //lower center
            {
                for (int l = 0; l < 5; l++)
                {
                    if (z + 1 < layers.Length && layers[z + 1][4][l] == true)
                    {
                        neighbours++;
                    }
                }
            }

            if (y == 2 && x == 1) //left center
            {
                for (int l = 0; l < 5; l++)
                {
                    if (z + 1 < layers.Length && layers[z + 1][l][0] == true)
                    {
                        neighbours++;
                    }
                }
            }

            if (y == 2 && x == 3) //right center
            {
                for (int l = 0; l < 5; l++)
                {
                    if (z + 1 < layers.Length && layers[z + 1][l][4] == true)
                    {
                        neighbours++;
                    }
                }
            }

            return neighbours;
        }

        static bool[][] NewLayer()
        {
            bool[][] layer = new bool[5][];
            for (int i = 0; i < layer.Length; i++)
            {
                layer[i] = new bool[5];
            }
            return layer;
        }

        static int Neighbours(char[][] data, int i, int j)
        {
            int neighbours = 0;
            if (i - 1 >= 0 && data[i - 1][j] == '#')
            {
                neighbours++;
            }
            if (i + 1 < 5 && data[i + 1][j] == '#')
            {
                neighbours++;
            }
            if (j - 1 >= 0 && data[i][j - 1] == '#')
            {
                neighbours++;
            }
            if (j + 1 < 5 && data[i][j + 1] == '#')
            {
                neighbours++;
            }
            return neighbours;
        }

        static long Rating(char[][] data)
        {
            long rating = 0;
            int index = 0;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (data[i][j] == '#')
                    {
                        rating += (long)Math.Pow(2, index);
                    }
                    index++;
                }
            }

            return rating;
        }

        static int GetHash(char[][] data)
        {
            int hash = 0;
            int index = 0;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (data[i][j] == '#')
                    {
                        hash |= 1 << index;
                    }
                    index++;
                }
            }

            return hash;
        }

        static void PrintScreen(char[][] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                string buffer = "";
                for (int j = 0; j < items[i].Length; j++)
                {
                    buffer += items[i][j];
                }
                Console.WriteLine(buffer);
            }
        }
    }
}
