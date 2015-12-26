using System;

public class Solution
{
    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int c = int.Parse(Console.ReadLine());
        int[] budgets = new int[n];
        long total = 0;
        for (int i = 0; i < n; i++)
        {
            int b = int.Parse(Console.ReadLine());
            total += b;
            budgets[i] = b;
        }
        if (total < c) { Console.WriteLine("IMPOSSIBLE"); return; }

        Array.Sort(budgets);
        int toPay = c, remaining = n;
        for (int i = 0; i < n - 1; i++, remaining--)
        {
            int budget = budgets[i];
            if (toPay > budget * remaining)
            {
                Console.WriteLine(budget);
                toPay -= budget;
            }
            else
            {
                int cost = toPay / remaining;
                Console.WriteLine(cost);
                toPay -= cost;
            }
        }
        
        Console.WriteLine(toPay);
    }
}