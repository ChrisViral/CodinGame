using System;
using System.Collections.Generic;

public class Solution
{
    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());
        string[] inputs = Console.ReadLine().Split(' ');
        List<int> maximums = new List<int>();
        int prev = int.Parse(inputs[0]), curr = int.Parse(inputs[1]), next = 0;
        if (prev > curr) { maximums.Add(prev); }
        int maxDiff = 0;
        for (int i = 2; i < N; i++)
        {
            next = int.Parse(inputs[i]);
            if (curr == next) { continue; }
            if (IsMax(prev, curr, next))
            {
                maximums.Add(curr);
            }
            else if (IsMin(prev, curr, next))
            {
                GetMaxDiff(curr, maximums, ref maxDiff);
            }
            prev = curr;
            curr = next;
        }
        if (curr < prev) { GetMaxDiff(curr, maximums, ref maxDiff); }

        Console.WriteLine(maxDiff);
    }

    public static bool IsMax(int prev, int curr, int next)
    {
        return curr > prev && curr > next;
    }

    public static bool IsMin(int prev, int curr, int next)
    {
        return curr < prev && curr < next;
    }

    public static void GetMaxDiff(int min, List<int> maximums, ref int maxDiff)
    {
        for (int j = 0; j < maximums.Count; j++)
        {
            int max = maximums[j];
            int diff = min - max;
            if (diff < maxDiff) { maxDiff = diff; }
        }
    }
}