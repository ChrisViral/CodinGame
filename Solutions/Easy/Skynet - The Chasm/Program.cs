using System;

public class Player
{
    private static void Main()
    {
        int road = int.Parse(Console.ReadLine());       //The length of the road before the gap.
        int gap = int.Parse(Console.ReadLine());        //The length of the gap.
        int platform = int.Parse(Console.ReadLine());   //The length of the landing platform.

        //Game loop
        while (true)
        {
            int speed = int.Parse(Console.ReadLine());  //The motorbike's speed.
            int coordX = int.Parse(Console.ReadLine()); //The position on the road of the motorbike.
            
            if (coordX < road)
            {
                if (speed <= gap) { Console.WriteLine("SPEED"); }
                else if (speed > gap + 1) { Console.WriteLine("SLOW"); }
                else if (coordX == road - 1) { Console.WriteLine("JUMP"); }
                else { Console.WriteLine("WAIT"); }
            }
            else { Console.WriteLine("SLOW"); }
        }
    }
}