using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode8
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string input;

            using (StreamReader sr = new StreamReader(File.OpenRead("data.txt")))
            {
                input = sr.ReadToEnd();
            }

            int width = 25, height = 6;

            List<string> layers = new List<string>();
            for (int i = 0; i < input.Length; i += width * height)
            {
                layers.Add(input.Substring(i, width * height));
            }

            Console.WriteLine($"Layers: {layers.Count}");

            // Part 1

            List<int> digits_counted = new List<int>();

            foreach (string layer in layers)
            {
                int zerodigits = layer.Count(c => c == '0');
                digits_counted.Add(zerodigits);
            }

            int index = digits_counted.IndexOf(digits_counted.Min());

            Console.WriteLine($"Least zero digits: Layer {index}");

            int result1 = layers[index].Count(c => c == '1') * layers[index].Count(c => c == '2');
            Console.WriteLine($"Result: {result1}");

            // Part 2

            int[][] pixels = new int[height][];
            const int pixelblack = 1, pixelwhite = 2;

            for (int i = 0; i < height; i++)
            {
                pixels[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    foreach (string layer in layers)
                    {
                        if (layer[i * width + j] == '0' && pixels[i][j] != pixelwhite) //black
                        {
                            pixels[i][j] = pixelblack;
                        }
                        if (layer[i * width + j] == '1' && pixels[i][j] != pixelblack) //white
                        {
                            pixels[i][j] = pixelwhite;
                        }
                        //if (layer[i * width + j] == '2') //transparent    //just ignore them
                        //{
                        //    pixels[i][j] = 2;
                        //}
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                string buffer = "";
                for (int j = 0; j < width; j++)
                {
                    if (pixels[i][j] == pixelblack)
                        buffer += ' ';
                    if (pixels[i][j] == pixelwhite)
                        buffer += '█';
                }
                Console.WriteLine(buffer);
            }

            Console.ReadKey();
        }
    }
}
