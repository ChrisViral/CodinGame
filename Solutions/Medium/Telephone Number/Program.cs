using System;
using System.Linq;
using System.Collections.Generic;


public class Solution
{
    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());

        if (N == 0) { Console.WriteLine("0"); return; }
        if (N == 1) { Console.WriteLine(Console.ReadLine().Length); return; }
        
        string[] numbers = new string[N];
        for (int i = 0; i < N; i++)
        {
            numbers[i] = Console.ReadLine();
        }

        Console.WriteLine(GetAmount(numbers, 0)); // The number of elements (referencing a number) stored in the structure.
    }

    public static int GetAmount(IEnumerable<string> numbers, int index)
    {
        int length = index + 1;
        int amount = 0;
        //Sometimes I love LINQ
        foreach (IGrouping<char, string> group in numbers.GroupBy(n => n[index]))
        {
            amount += 1 + GetAmount(group.Where(n => n.Length > length), length);
        }
        return amount;
    }
}