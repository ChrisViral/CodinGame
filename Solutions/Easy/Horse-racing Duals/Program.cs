using System;

public class Solution
{
    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int[] horses = new int[n];
        for (int i = 0; i < n; i++)
        {
            horses[i] = int.Parse(Console.ReadLine());
        }

        Array.Sort(horses);
        int diff = Math.Abs(horses[0] - horses[1]);
        for (int i = 1; i < n - 1;)
        {
            int d = Math.Abs(horses[i++] - horses[i]);
            if (d < diff) { diff = d; }
        }

        Console.WriteLine(diff);
    }
}