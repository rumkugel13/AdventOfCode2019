using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Point3D[] moons = new Point3D[4];
            using (StreamReader streamReader = new StreamReader(File.OpenRead("data.txt")))
            {
                int i = 0;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    line = line.Substring(1, line.Length - 2);
                    var split = line.Split(", ");
                    moons[i] = new Point3D(long.Parse(split[0].Substring(2)), long.Parse(split[1].Substring(2)), long.Parse(split[2].Substring(2)));
                    i++;
                }
            }

            //Point3D[] moons = new Point3D[4];   // test1.1: 10 steps | 2772 steps
            //moons[0] = new Point3D(-1, 0, 2);
            //moons[1] = new Point3D(2, -10, -7);
            //moons[2] = new Point3D(4, -8, 8);
            //moons[3] = new Point3D(3, 5, -1);

            //Point3D[] moons = new Point3D[4];   // test1.2: 1000 stepss
            //moons[0] = new Point3D(-8, -10, 0);
            //moons[1] = new Point3D(5, 5, 10);
            //moons[2] = new Point3D(2, -7, 3);
            //moons[3] = new Point3D(9, -8, -3);

            Point3D[] velocities = new Point3D[4];
            
            long[][] initialState = GetState(moons, velocities);    //states stored in axis first      

            string[] initialStrings = new string[3];
            initialStrings[0] = StateToString(initialState[0]);
            initialStrings[1] = StateToString(initialState[1]);
            initialStrings[2] = StateToString(initialState[2]);

            long[] cycles = new long[3];  //cycles for x,y,z

            long result1 = 0;

            long timesteps = long.MaxValue;
            for (long i = 1; i < timesteps; i++)
            {
                //Console.WriteLine($"Step {i-1}: {moons[0].ToString()} , {velocities[0].ToString()}");
                //Console.WriteLine($"Step {i-1}: {moons[1].ToString()} , {velocities[1].ToString()}");
                //Console.WriteLine($"Step {i-1}: {moons[2].ToString()} , {velocities[2].ToString()}");
                //Console.WriteLine($"Step {i-1}: {moons[3].ToString()} , {velocities[3].ToString()}");

                // get gravity from every pair of moons and add to velocity
                for (int j = 0; j < moons.Length; j++)
                {
                    for (int k = j; k < moons.Length; k++)
                    {
                        if (moons[j].X < moons[k].X)
                        {
                            velocities[j].X++;
                            velocities[k].X--;
                        }
                        else if (moons[j].X > moons[k].X)
                        {
                            velocities[j].X--;
                            velocities[k].X++;
                        }

                        if (moons[j].Y < moons[k].Y)
                        {
                            velocities[j].Y++;
                            velocities[k].Y--;
                        }
                        else if (moons[j].Y > moons[k].Y)
                        {
                            velocities[j].Y--;
                            velocities[k].Y++;
                        }

                        if (moons[j].Z < moons[k].Z)
                        {
                            velocities[j].Z++;
                            velocities[k].Z--;
                        }
                        else if (moons[j].Z > moons[k].Z)
                        {
                            velocities[j].Z--;
                            velocities[k].Z++;
                        }
                    }
                }

                // apply velocity to position
                for (int j = 0; j < moons.Length; j++)
                {
                    moons[j].Add(velocities[j]);
                }

                if (i == 1000)  // result1 after 1000 steps, get the energy
                {
                    result1 = moons[0].AbsoluteSum() * velocities[0].AbsoluteSum();
                    result1 += moons[1].AbsoluteSum() * velocities[1].AbsoluteSum();
                    result1 += moons[2].AbsoluteSum() * velocities[2].AbsoluteSum();
                    result1 += moons[3].AbsoluteSum() * velocities[3].AbsoluteSum();
                }

                long[][] newState = GetState(moons, velocities);    //states stored in axis first      

                string[] newStrings = new string[3];
                newStrings[0] = StateToString(newState[0]);
                newStrings[1] = StateToString(newState[1]);
                newStrings[2] = StateToString(newState[2]);

                for (int a = 0; a < 3; a++)
                {
                    if (cycles[a] == 0 && initialStrings[a] == newStrings[a])
                    {
                        cycles[a] = i;
                    }
                }

                if (cycles[0] != 0 && cycles[1] != 0 && cycles[2] != 0)
                {
                    break; //found all cylces, break;
                }
            }

            long result2 = LCM(cycles[0], LCM(cycles[1], cycles[2]));   //use all individual cycles to find supercycle

            Console.WriteLine($"Result: {result1}, {result2}");

            // results: 12773, 306798770391636

            Console.ReadKey();
        }

        static string StateToString(long[] state)   // string for one axis state
        {
            return $"<{state[0]},{state[1]},{state[2]}>,<{state[3]},{state[4]},{state[5]}>";
        }

        static long[][] GetState(Point3D[] moons, Point3D[] velocities)
        {
            long[][] state = new long[3][];

            for (int i = 0; i < state.Length; i++)
            {
                state[i] = new long[6];
            }

            state[0][0] = moons[0].X;
            state[0][1] = moons[1].X;
            state[0][2] = moons[2].X;
            state[0][3] = velocities[0].X;
            state[0][4] = velocities[1].X;
            state[0][5] = velocities[2].X;

            state[1][0] = moons[0].Y;
            state[1][1] = moons[1].Y;
            state[1][2] = moons[2].Y;
            state[1][3] = velocities[0].Y;
            state[1][4] = velocities[1].Y;
            state[1][5] = velocities[2].Y;

            state[2][0] = moons[0].Z;
            state[2][1] = moons[1].Z;
            state[2][2] = moons[2].Z;
            state[2][3] = velocities[0].Z;
            state[2][4] = velocities[1].Z;
            state[2][5] = velocities[2].Z;

            return state;
        }

        struct Point3D
        {
            public long X;
            public long Y;
            public long Z;

            public Point3D(long x, long y, long z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public void Add(Point3D point)
            {
                X += point.X;
                Y += point.Y;
                Z += point.Z;
            }

            public long AbsoluteSum()
            {
                return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            }

            public override string ToString()
            {
                return $"<{X},{Y},{Z}>";
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (!(obj is Point3D)) return false;

                Point3D point = (Point3D)obj;

                return X == point.X && Y == point.Y && Z == point.Z;
            }

            public override int GetHashCode()
            {
                return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
            }
        }

        // Use Euclid's algorithm to calculate the
        // greatest common divisor (GCD) of two numbers.
        private static long GCD(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            // Pull out remainders.
            for (; ; )
            {
                long remainder = a % b;
                if (remainder == 0) return b;
                a = b;
                b = remainder;
            };
        }

        // Return the least common multiple
        // (LCM) of two numbers.
        private static long LCM(long a, long b)
        {
            return a * b / GCD(a, b);
        }
    }
}
