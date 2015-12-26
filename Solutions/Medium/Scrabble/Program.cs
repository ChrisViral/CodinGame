using System;
using System.Linq;
using System.Collections.Generic;

public class Solution
{
    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        string[] words = new string[n];
        Dictionary<char, int>[] charCount = new Dictionary<char, int>[n];
        for (int i = 0; i < n; i++)
        {
            string word = Console.ReadLine();
            words[i] = word;
            charCount[i] = word.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        }
        Dictionary<char, int> letters = Console.ReadLine().GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

        string best = "git gud";
        int points = 0;
        for (int i = 0; i < n; i++)
        {
            Dictionary<char, int> word = charCount[i];
            int count = 0;
            if (word.All(p => letters.TryGetValue(p.Key, out count) && count >= p.Value))
            {
                int p = GetAllPoints(word);
                if (p > points)
                {
                    points = p;
                    best = words[i];
                }
            }
        }

        Console.WriteLine(best);
    }

    public static int GetAllPoints(Dictionary<char, int> chars)
    {
        return chars.Sum(p => GetPoints(p.Key) * p.Value);
    }

    public static int GetPoints(char c)
    {
        switch(c)
        {
            case 'a':
            case 'e':
            case 'i':
            case 'l':
            case 'n':
            case 'o':
            case 'r':
            case 's':
            case 't':
            case 'u':
                return 1;

            case 'd':
            case 'g':
                return 2;

            case 'b':
            case 'c':
            case 'm':
            case 'p':
                return 3;

            case 'f':
            case 'h':
            case 'v':
            case 'w':
            case 'y':
                return 4;

            case 'k':
                return 5;

            case 'j':
            case 'x':
                return 8;

            case 'q':
            case 'z':
                return 10;

            default:
                return 0;
        }
    }
}