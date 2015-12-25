using System;

public class Player
{
    private static void Main()
    {
        //Initiate variables
        string e1, e2;
        int d1, d2;

        //Game loop
        while (true)
        {
            e1 = Console.ReadLine();            //Enemy 1
            d1 = int.Parse(Console.ReadLine()); //Distance 1
            e2 = Console.ReadLine();            //Enemy 2
            d2 = int.Parse(Console.ReadLine()); //Distance 2

            Console.WriteLine(d1 < d2 ? e1 : e2);
        }
    }
}