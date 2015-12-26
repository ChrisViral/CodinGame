using System;

public class Player
{
    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);   //Number of columns.
        int height = int.Parse(inputs[1]);  //Number of rows.

        int[,] grid = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            inputs = Console.ReadLine().Split(' '); // represents a line in the grid and contains W integers. Each integer represents one room of a given type.
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = int.Parse(inputs[j]);
            }
        }
        int exit = int.Parse(Console.ReadLine()); //The coordinate along the X axis of the exit (not useful for this first mission, but must be read).

        //Game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            string POS = inputs[2];
            
            int room = grid[y, x];
            switch (room)
            {
                case 1:
                case 3:
                case 7:
                case 8:
                case 9:
                case 12:
                case 13:
                    y++; break;

                case 2:
                case 6:
                    {
                        switch (POS)
                        {
                            case "LEFT":
                                x++; break;

                            case "RIGHT":
                                x--; break;
                        }
                        break;
                    }

                case 4:
                    {
                        switch (POS)
                        {
                            case "TOP":
                                x--; break;

                            case "RIGHT":
                                y++; break;
                        }
                        break;
                    }

                case 5:
                    {
                        switch (POS)
                        {
                            case "TOP":
                                x++; break;

                            case "LEFT":
                                y++; break;
                        }
                        break;
                    }

                case 10:
                    x--; break;

                case 11:
                    x++; break;
            }

            Console.WriteLine(x + " " + y);
        }
    }
}