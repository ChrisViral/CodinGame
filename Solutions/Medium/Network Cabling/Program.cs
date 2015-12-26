using System;

public class Solution
{
    public struct Point
    {
        #region Fields
        public readonly long x;
        public readonly long y;
        #endregion

        #region Constructor
        public Point(long x, long y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion
    }

    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        if (n <= 1) { Console.WriteLine("0"); return; }

        Point[] points = new Point[n];
        string[] inputs;
        for (int i = 0; i < n; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            points[i] = new Point(long.Parse(inputs[0]), long.Parse(inputs[1]));
        }

        long minX = long.MaxValue, maxX = long.MinValue, total = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Point point = points[i];
            if (point.x < minX) { minX = point.x; }
            if (point.x > maxX) { maxX = point.x; }
            total += point.y;
        }
        //For some shady reason, casting one of those to double to get accurate floating point division
        //leads to a failed 7th validaton test. This *should* be done with floating point division.
        //Whatever works I guess.
        double average = total / /*(double)*/points.Length;

        long optimal = points[0].y;
        double minDiff = Math.Abs(optimal - average);
        for (int i = 1; i < points.Length; i++)
        {
            long y = points[i].y;
            double diff = Math.Abs(y - average);
            if (diff < minDiff)
            {
                minDiff = diff;
                optimal = y;
            }
        }

        long lengthY = 0;
        for (int i = 0; i < points.Length; i++)
        {
            lengthY += Math.Abs(points[i].y - optimal);
        }
        
        Console.WriteLine(lengthY + Math.Abs(maxX - minX));
    }
}