using System;

public class Player
{
    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int nbFloors = int.Parse(inputs[0]);        //Number of floors
        int width = int.Parse(inputs[1]);           //Width of the area
        int nbRounds = int.Parse(inputs[2]);        //Maximum number of rounds
        int exitFloor = int.Parse(inputs[3]);       //Floor on which the exit is found
        int exitPos = int.Parse(inputs[4]);         //Position of the exit on its floor
        int nbTotalClones = int.Parse(inputs[5]);   //Number of generated clones
        //int nbAdditionalElevators = int.Parse(inputs[6]); //Ignore (always zero)
        int nbElevators = int.Parse(inputs[7]);     //Number of elevators

        int[] elevators = new int[nbElevators];
        for (int i = 0; i < nbElevators; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            elevators[int.Parse(inputs[0])] = int.Parse(inputs[1]);
        }

        //Game loop
        int maxFloor = -1;
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]);  //Floor of the leading clone
            int clonePos = int.Parse(inputs[1]);    //Position of the leading clone on its floor
            string direction = inputs[2];           //Direction of the leading clone: LEFT or RIGHT

            if (clonePos == 0 || clonePos == width - 1) { Console.WriteLine("BLOCK"); }
            else if (cloneFloor == exitFloor)
            {
                if (cloneFloor > 0  && clonePos == elevators[cloneFloor - 1])
                {
                    Console.WriteLine("WAIT");
                }
                else if ((clonePos < exitPos && direction == "LEFT") || (clonePos > exitPos && direction == "RIGHT"))
                {
                    Console.WriteLine("BLOCK");
                }
                else { Console.WriteLine("WAIT"); }
            }
            else if (cloneFloor > maxFloor && (cloneFloor == 0 || clonePos != elevators[cloneFloor - 1]))
            {
                int elevatorPos = elevators[cloneFloor];
                if ((clonePos < elevatorPos && direction == "LEFT") || (clonePos > elevatorPos && direction == "RIGHT"))
                {
                    maxFloor = cloneFloor;
                    Console.WriteLine("BLOCK");
                }
                else { Console.WriteLine("WAIT"); }
            }
            else { Console.WriteLine("WAIT"); }
        }
    }
}