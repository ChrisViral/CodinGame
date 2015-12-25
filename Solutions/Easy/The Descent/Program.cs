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
    }

    #region Methods
    private static void Main()
    {
        int[] mountains = new int[8];
        string[] inputs;
        Point pos;

        //Game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            pos = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            for (int i = 0; i < 8; i++)
            {
                mountains[i] = int.Parse(Console.ReadLine());
            }

            Console.WriteLine(pos.x == GetHighest(mountains) ? "FIRE" : "HOLD");
        }
    }
    
    public static int GetHighest(int[] mountains)
    {
        int length = mountains.Length;
        if (length == 0) { return -1; }
        int index = 0, max = mountains[0];
        for (int i = 1; i < length; i++)
        {
            int m = mountains[i];
            if (m > max)
            {
                index = i;
                max = m;
            }
        }
        return index;
    }
    #endregion
}