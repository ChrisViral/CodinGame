using System;
using System.Linq;

public class Solution
{
    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());  //The number of temperatures to analyse
        if (n == 0) { Console.WriteLine("0"); return; }
        string[] inputs = Console.ReadLine().Split(' ');
        int[] temps = new int[n];
        for (int i = 0; i < n; i++)
        {
            temps[i] = int.Parse(inputs[i]);
        }

        int c = temps[0], cAbs = Math.Abs(c);       
        for (int i = 1; i < n; i++)
        {
            int t = temps[i];
            if (t == 0) { Console.WriteLine("0"); return; }

            int a = Math.Abs(t);
            if (a < cAbs || (a == cAbs && t > 0))
            {
                c = t;
                cAbs = Math.Abs(t);
            }
        }

        Console.WriteLine(c);
    }
}