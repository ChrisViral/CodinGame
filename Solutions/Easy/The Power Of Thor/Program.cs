using System;

public class Player
{
    public struct Point
    {
        #region Fields
        public readonly int x;
        public readonly int y;
        #endregion

        #region Constructor
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Method
        public Point MoveTowards(Point target, out string direction)
        {
            direction = string.Empty;
            int dx = this.x, dy = this.y;
            if (this.y > target.y) { dy--; direction = "N"; }
            else if (this.y < target.y) { dy++; direction = "S"; }
            if (this.x > target.x) { dx--; direction += "W"; }
            else if (this.x < target.x) { dx++; direction += "E"; }
            return new Point(dx, dy);
        }
        #endregion
    }

    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        Point light = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
        Point thor = new Point(int.Parse(inputs[2]), int.Parse(inputs[3]));
        string direction;
        int remainingTurns;

        //Game loop
        while (true)
        {
            remainingTurns = int.Parse(Console.ReadLine()); //The remaining amount of turns Thor can move
            thor = thor.MoveTowards(light, out direction);
            Console.WriteLine(direction);
        }
    }
}