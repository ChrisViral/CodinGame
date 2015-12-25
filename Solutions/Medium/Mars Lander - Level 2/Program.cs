using System;

public class Player
{
    #region Constants
    public const int maxHorizontal = 20;
    public const int maxVertical = 40;
    public const double g = 3.711;
    public const double radToDeg = 180 / Math.PI;
    #endregion

    public struct Vector
    {
        #region Fields
        public readonly int x;
        public readonly int y;
        #endregion

        #region Properties
        public int sqrMag
        {
            get { return (x * x) + (y * y); }
        }

        public double mag
        {
            get { return Math.Sqrt(sqrMag); }
        }
        #endregion

        #region Constructor
        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion
    }

    #region Methods
    private static void Main()
    {
        int surfaceN = int.Parse(Console.ReadLine()); // the number of points used to draw the surface of Mars.

        string[] inputs = Console.ReadLine().Split(' ');
        int[] last = new int[] { int.Parse(inputs[0]), int.Parse(inputs[1]) };
        int minX = 0, maxX = 0;
        int alt = 0;
        for (int i = 1; i < surfaceN; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int landX = int.Parse(inputs[0]); //X coordinate of a surface point. (0 to 6999)
            int landY = int.Parse(inputs[1]); //Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.
            if (landY == last[1])
            {
                minX = last[0];
                maxX = landX;
                alt = landY;
            }
            last[0] = landX;
            last[1] = landY;
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

            Vector velocity = new Vector(hSpeed, vSpeed);
            int R = 0, P = 0;

            if (OverFlat(X, minX, maxX))
            {
                if (Landing(Y, alt))
                {
                    P = 3;
                }
                else if (!CheckSpeed(velocity))
                {
                    R = SlowAngle(velocity);
                    P = 4;
                }
            }
            else
            {
                if (WrongDirection(X, hSpeed, minX, maxX) || TooFast(hSpeed))
                {
                    R = SlowAngle(velocity);
                    P = 4;
                }
                else if (TooSlow(hSpeed))
                {
                    R = FastAngle(X, minX);
                    P = 4;
                }
                else
                {
                    R = 0;
                    P = vSpeed >= 1 ? 3 : 4;
                }
            }

            Console.WriteLine(R + " " + P);
        }
    }

    public static bool OverFlat(int position, int min, int max)
    {
        return position >= min && position < max;
    }

    public static bool Landing(int alt, int landingAlt)
    {
        return alt <= landingAlt + 50;
    }

    public static bool CheckSpeed(Vector velocity)
    {
        return Math.Abs(velocity.x) < 1 && Math.Abs(velocity.y) < maxVertical - 5;
    }

    public static int SlowAngle(Vector velocity)
    {
        return (int)(Math.Asin(velocity.x / velocity.mag) * radToDeg);
    }

    public static int FastAngle(int pos, int min)
    {
        return (int)(Math.Acos(g / 4d) * radToDeg) * (pos < min ? -1 : 1);
    }

    public static bool WrongDirection(int pos, int horizontal, int min, int max)
    {
        return (pos < min && horizontal < 0) || (pos > max && horizontal > 0);
    }

    public static bool TooSlow(int horizontal)
    {
        return Math.Abs(horizontal) <= maxHorizontal * 2;
    }

    public static bool TooFast(int horizontal)
    {
        return Math.Abs(horizontal) > maxHorizontal * 4;
    }

    public static bool Approaching(int vertical, int pos, int max, int min)
    {
        return (vertical > 0 && pos + 100 >= min) || (vertical < 0 && pos - 100 <= max);
    }
    #endregion
}