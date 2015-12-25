using System;

public class Solution
{
    public const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    
    private static void Main()
    {
        int L = int.Parse(Console.ReadLine());
        int H = int.Parse(Console.ReadLine());
        string text = Console.ReadLine().ToUpper();
        
        for (int i = 0; i < H; i++)
        {
            string row = Console.ReadLine();
            string t = string.Empty;
            for (int j = 0; j < text.Length; j++)
            {
                int ind = letters.IndexOf(text[j]);
                t += row.Substring((ind == -1 ? 26 : ind) * L, L);
            }
            Console.WriteLine(t);
        }
    }
}