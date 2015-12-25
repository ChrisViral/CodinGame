using System;

public class Player
{
    private static void Main()
    {
        int surfaceN = int.Parse(Console.ReadLine());   //The number of points used to draw the surface of Mars.
        string[] inputs;
        for (int i = 0; i < surfaceN; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int landY = int.Parse(inputs[1]);
            int landX = int.Parse(inputs[0]);
        }

        //Game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int X = int.Parse(inputs[0]);
            int Y = int.Parse(inputs[1]);
            int hSpeed = int.Parse(inputs[2]);  //The horizontal speed (in m/s), can be negative.
            int vSpeed = int.Parse(inputs[3]);  //The vertical speed (in m/s), can be negative.
            int fuel = int.Parse(inputs[4]);    //The quantity of remaining fuel in liters.
            int rotate = int.Parse(inputs[5]);  //The rotation angle in degrees (-90 to 90).
            int power = int.Parse(inputs[6]);   //The thrust power (0 to 4).

            Console.WriteLine(Y < 2235 && vSpeed < -39 ? "0 4" : "0 0");
        }
    }
}