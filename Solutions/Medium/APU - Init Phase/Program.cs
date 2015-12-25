using System;

public class Player
{
    private static void Main()
    {
        int width = int.Parse(Console.ReadLine());  //The number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); //Tthe number of cells on the Y axis

        bool[,] grid = new bool[height, width];
        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine(); // width characters, each either 0 or .
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = line[j] == '0';
            }
        }
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[i, j])
                {
                    string s = j + " " + i + " ";
                    bool found = false;
                    for (int k = j + 1; k < width; k++)
                    {
                        if (grid[i, k])
                        {
                            s += k + " " + i + " ";
                            found = true;
                            break;
                        }
                    }
                    if (!found) { s += "-1 -1 "; }
                    
                    found = false;
                    for (int k = i + 1; k < height; k++)
                    {
                        if (grid[k, j])
                        {
                            s += j + " " + k;
                            found = true;
                            break;
                        }
                    }
                    if (!found) { s += "-1 -1"; }
                    
                    Console.WriteLine(s);
                }
            }
        }
    }
}