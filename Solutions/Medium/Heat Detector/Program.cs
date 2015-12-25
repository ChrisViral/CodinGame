using System;
using System.Linq;
using System.Collections.Generic;

public class Player
{
    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int W = int.Parse(inputs[0]);           //Width of the building.
        int H = int.Parse(inputs[1]);           //Height of the building.
        int N = int.Parse(Console.ReadLine());  //Maximum number of turns before game over.
        inputs = Console.ReadLine().Split(' ');
        int x = int.Parse(inputs[0]);
        int y = int.Parse(inputs[1]);

        List<int> floors = new List<int>(H);
        List<int> positions = new List<int>(W);
        for (int i = 0; ; i++)
        {
            if (i < H)
            {
                floors.Add(i);
                if (i < W) { positions.Add(i); }
            }
            else if (i < W) { positions.Add(i); }
            else { break; }
        }
        
        // game loop
        while (true)
        {
            string direction = Console.ReadLine(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)

            switch (direction)
            {
                case "U":
                    {
                        floors.RemoveAll(f => f >= y);
                        positions.RemoveAll(p => p != x);
                        break;
                    }
                case "UR":
                    {
                        floors.RemoveAll(f => f >= y);
                        positions.RemoveAll(p => p <= x);
                        break;
                    }
                case "R":
                    {
                        floors.RemoveAll(f => f != y);
                        positions.RemoveAll(p => p <= x);
                        break;
                    }
                case "DR":
                    {
                        floors.RemoveAll(f => f <= y);
                        positions.RemoveAll(p => p <= x);
                        break;
                    }
                case "D":
                    {
                        floors.RemoveAll(f => f <= y);
                        positions.RemoveAll(p => p != x);
                        break;
                    }
                case "DL":
                    {
                        floors.RemoveAll(f => f <= y);
                        positions.RemoveAll(p => p >= x);
                        break;
                    }
                case "L":
                    {
                        floors.RemoveAll(f => f != y);
                        positions.RemoveAll(p => p >= x);
                        break;
                    }
                case "UL":
                    {
                        floors.RemoveAll(f => f >= y);
                        positions.RemoveAll(p => p >= x);
                        break;
                    }
            }

            x = (positions.Max() + positions.Min()) / 2;
            y = (floors.Max() + floors.Min()) / 2;

            Console.WriteLine(x + " " + y); // the location of the next window Batman should jump to.
        }
    }
}